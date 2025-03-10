using Microsoft.AspNetCore.Mvc;
using SocketheadCleanArch.API.Authentication;
using SocketheadCleanArch.API.Extensions;
using SocketheadCleanArch.API.Models;

namespace SocketheadCleanArch.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(UserAuthService userAuthService) : ControllerBase
{
    /// <summary>
    /// Authenticate against email/password
    /// Returns 200 on success along with a JWT Access Token and user details
    /// Returns 401 if login failed (user not found or password did not match)
    /// </summary>
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
}
