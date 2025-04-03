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
    UserAuthService userAuthService,
    ILogger<AuthController> logger)
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
        string? redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl })!;
        AuthenticationProperties? properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return Challenge(properties, provider);
    }

    /// <summary>
    /// Callback handler for external providers. Generates JWT token for the logged-in user.
    /// </summary>
    [HttpGet("external-login-callback")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExternalLoginCallback([FromQuery] string returnUrl = "/")
    {
        ExternalLoginInfo? loginInfo = await signInManager.GetExternalLoginInfoAsync();
        if (loginInfo is null)
            return Redirect("/login?error=ExternalLoginFailed");

        AuthResponse? response = await userAuthService.AuthenticateExternalAsync(loginInfo);
        if (!string.IsNullOrEmpty(response.FailureReason))
            return Unauthorized(response.FailureReason);

        return Ok(response);
    }
}
