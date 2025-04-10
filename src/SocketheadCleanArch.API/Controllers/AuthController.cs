using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocketheadCleanArch.API.Authentication;
using SocketheadCleanArch.API.Extensions;
using SocketheadCleanArch.API.Models;
using SocketheadCleanArch.Domain.Entities;

namespace SocketheadCleanArch.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    SignInManager<AppUser> signInManager,
    UserAuthService userAuthService)
    : ControllerBase
{

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(AuthRequest authRequest)
    {
        AuthResponse authResponse = await userAuthService.AuthenticateUserAsync(
            email: authRequest.Email, 
            password: authRequest.Password);

        return authResponse.IsSuccess 
            ? this.OkResponse(authResponse)
            : this.NotAuthorizedResponse(detail: authResponse.FailureReason ?? "Not Authorized");
    }
    
    [HttpGet("external-login")]
    public IActionResult ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl = "/")
    {
        if (string.IsNullOrWhiteSpace(provider))
            return BadRequest("Provider is required.");

        string? redirectUrl = userAuthService.GetExternalLoginRedirectUrl(provider, returnUrl, Request);

        if (string.IsNullOrEmpty(redirectUrl))
            return BadRequest("Unsupported provider or invalid configuration.");

        if (provider.Equals("apple", StringComparison.OrdinalIgnoreCase))
            return Redirect(redirectUrl);

        AuthenticationProperties? properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }
    

    /// <summary>
    /// Callback handler for external providers. Generates JWT token for the logged-in user.
    /// </summary>
    [HttpGet("external-login-callback/google")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExternalLoginCallbackGoogle([FromQuery] string returnUrl = "/")
    {
        ExternalLoginInfo? loginInfo = await signInManager.GetExternalLoginInfoAsync();
        if (loginInfo is null)
            return Redirect("/login?error=ExternalLoginFailed");

        AuthResponse response = await userAuthService.AuthenticateGoogleAsync(loginInfo);
        if (!string.IsNullOrEmpty(response.FailureReason))
            return Unauthorized(response.FailureReason);

        return Ok(response);
    }
    
    [HttpPost("external-login-callback/apple")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExternalLoginCallbackApple([FromForm] string returnUrl = "/")
    {
        string? idToken = Request.Form["id_token"];
        if (string.IsNullOrWhiteSpace(idToken))
            return BadRequest("Missing Apple ID token.");

        AuthResponse response = await userAuthService.AuthenticateAppleAsync(idToken);
        if (!string.IsNullOrEmpty(response.FailureReason))
            return Unauthorized(response.FailureReason);

        return Ok(response);
    }


}
