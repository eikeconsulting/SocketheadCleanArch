using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SocketheadCleanArch.API.Models;
using SocketheadCleanArch.Domain.Dtos;
using SocketheadCleanArch.Domain.Entities;
using SocketheadCleanArch.Service.Repository;
using Microsoft.AspNetCore.Mvc;
using SocketheadCleanArch.API.Utils;


namespace SocketheadCleanArch.API.Authentication;

public class UserAuthService(UserAdminRepository userAdmin, JwtTokenService jwtTokenService, IConfiguration config,IUrlHelperFactory urlHelperFactory,IActionContextAccessor actionContextAccessor)
{
    public async Task<AuthResponse> AuthenticateUserAsync(string email, string password)
    {
        AppUser? user = await userAdmin.FindUserByEmailAsync(email);
        if (user == null)
            return new AuthResponse { FailureReason = "User does not exist or invalid credentials" };
        
        bool success = await userAdmin.AuthenticateUserAsync(user, password);
        if (!success)
            return new AuthResponse { FailureReason = "User does not exist or invalid credentials" };

        IReadOnlyList<string> roles = await userAdmin.GetUserRolesAsync(user);

        IReadOnlyList<Claim> claims = AuthHelper.CreateUserClaims(user, roles);
        
        AccessTokenResult result = jwtTokenService.GenerateJwtAccessToken(claims);
        
        return new AuthResponse
        {
            AccessToken = result.AccessToken,
            AccessTokenExpiry = result.Expiration,
            User = new UserDto(
                UserId: user.Id, 
                Email: user.Email!, 
                FirstName: user.FirstName, 
                LastName: user.LastName,
                Roles: roles)
        };
    }
    
    public async Task<AuthResponse> AuthenticateGoogleAsync(ExternalLoginInfo loginInfo)
    {
        AppUser? user = await userAdmin.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

        if (user == null)
        {
            string? email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                return new AuthResponse { FailureReason = "Email not available from external provider" };
            }

            user = new AppUser
            {
                Email = email,
                FirstName = loginInfo.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                LastName = loginInfo.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
                UserName = email
            };

            IdentityResult? creationResult = await userAdmin.CreateUserAsync(user);
            if (!creationResult.Succeeded)
            {
                return new AuthResponse { FailureReason = "User creation failed" };
            }

            IdentityResult? addLoginResult = await userAdmin.AddLoginAsync(user, loginInfo);
            if (!addLoginResult.Succeeded)
            {
                return new AuthResponse { FailureReason = "Failed to link external login" };
            }
        }

        IReadOnlyList<string>? roles = await userAdmin.GetUserRolesAsync(user);
        List<Claim>? claims = AuthHelper.CreateUserClaims(user, roles);
        AccessTokenResult? tokenResult = jwtTokenService.GenerateJwtAccessToken(claims);

        return new AuthResponse
        {
            AccessToken = tokenResult.AccessToken,
            AccessTokenExpiry = tokenResult.Expiration,
            User = new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, roles)
        };
    }
    
    public async Task<AuthResponse> AuthenticateAppleAsync(string idToken)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        JwtSecurityToken jwt = tokenHandler.ReadJwtToken(idToken);

        string? email = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value;
        string? subject = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject))
        {
            return new AuthResponse { FailureReason = "Invalid Apple ID token." };
        }

        // Optionally: Validate token signature and issuer using Apple's public keys
        bool isValid = await AuthHelper.ValidateAppleTokenSignatureAsync(idToken);
        if (!isValid)
        {
            return new AuthResponse { FailureReason = "Apple token signature validation failed." };
        }

        // Check if the user already exists
        AppUser? user = await userAdmin.FindUserByEmailAsync(email);
        if (user == null)
        {
            user = new AppUser
            {
                Email = email,
                UserName = email,
                FirstName = string.Empty,
                LastName = string.Empty
            };

            IdentityResult createResult = await userAdmin.CreateUserAsync(user);
            if (!createResult.Succeeded)
            {
                return new AuthResponse { FailureReason = "Failed to create Apple user." };
            }

            await userAdmin.AddLoginAsync(user, new UserLoginInfo("Apple", subject, "Apple"));
        }

        IReadOnlyList<string> roles = await userAdmin.GetUserRolesAsync(user);
        List<Claim> claims = AuthHelper.CreateUserClaims(user, roles);
        AccessTokenResult token = jwtTokenService.GenerateJwtAccessToken(claims);

        return new AuthResponse
        {
            AccessToken = token.AccessToken,
            AccessTokenExpiry = token.Expiration,
            User = new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, roles)
        };
    }
    
    public string? GetExternalLoginRedirectUrl(string provider, string returnUrl, HttpRequest request)
    {
        if (string.IsNullOrWhiteSpace(provider))
            return null;
        
        string url = string.Empty;
        
        // Apple-specific handling
        if (provider.Equals("apple", StringComparison.OrdinalIgnoreCase))
        {
            string baseUrl = config["Authentication:Apple:AuthorizationEndpoint"] ?? "https://appleid.apple.com/auth/authorize";
            string clientId = config["Authentication:Apple:ClientId"] ?? throw new ArgumentException("Apple ClientId is missing in configuration.");
            string scope = config["Authentication:Apple:Scope"] ?? "name email";
            string responseType = config["Authentication:Apple:ResponseType"] ?? "code id_token";
            string responseMode = config["Authentication:Apple:ResponseMode"] ?? "form_post";
            string state = Guid.NewGuid().ToString("N");

            url = $"{request.Scheme}://{request.Host}/auth/external-login-callback/apple?provider=apple&returnUrl={Uri.EscapeDataString(returnUrl)}";

            return $"{baseUrl}?" +
                   $"client_id={Uri.EscapeDataString(clientId)}" +
                   $"&redirect_uri={Uri.EscapeDataString(url)}" +
                   $"&response_type={Uri.EscapeDataString(responseType)}" +
                   $"&scope={Uri.EscapeDataString(scope)}" +
                   $"&response_mode={Uri.EscapeDataString(responseMode)}" +
                   $"&state={Uri.EscapeDataString(state)}";
            
        }
        else if (provider.Equals("google", StringComparison.OrdinalIgnoreCase))
        {
            // For Google and other standard external providers
            IUrlHelper? urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext!);
            url = urlHelper.Action(
                action: "ExternalLoginCallbackGoogle",
                controller: "Auth",
                values: new { returnUrl, provider }
            ) ?? "";
           
        }
    
        return url;
    }

    
   


    
}