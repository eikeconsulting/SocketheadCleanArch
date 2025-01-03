using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocketheadCleanArch.API.Extensions;
using SocketheadCleanArch.API.Models;

namespace SocketheadCleanArch.API.Controllers;

#if DEBUG

/// <summary>
/// These endpoints are for the client to test and validate the API
/// They are only available for "Debug" builds
/// (i.e. should be Development only as Production should be "Release" builds
/// </summary>
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    /// <summary>
    /// This should return the string "Pong" in the "Data" part of the response
    /// This allows you to see what a valid response looks like
    /// </summary>
    [HttpGet("ping")]
    public ActionResult<ApiResponse<string>> Ping()
    {
        return this.OkResponse<string>("Pong.");
    }

    /// <summary>
    /// This should return the string "Authorized Pong" in the "Data" part of the response
    /// This should only work for Authorized users and should return an Unauthorized ProblemDetails
    /// </summary>
    [Authorize]
    [HttpGet("ping-authorized")]
    public ActionResult<ApiResponse<string>> PingAuthorized()
    {
        return this.OkResponse<string>("Authorized Pong.");
    }

    /// <summary>
    /// This should return the string "Admin Pong" in the "Data" part of the response
    /// This should only work for users with the Admin role
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("ping-admin")]
    public ActionResult<ApiResponse<string>> PingAdmin()
    {
        return this.OkResponse<string>("Admin Pong.");
    }
    
    /// <summary>
    /// This will throw an exception with the message "This is a test exception."
    /// This should be caught by the global exception handler and return a ProblemDetail object
    /// </summary>
    [HttpGet("throw-exception")]
    public ActionResult<ApiResponse<bool>> ThrowException()
    {
        throw new Exception("This is a test exception.");
    }

    /// <summary>
    /// This should return a Bad Request with a ProblemDetail object and status code 400
    /// </summary>
    [HttpGet("bad-request")]
    public ActionResult<ApiResponse<bool>> _BadRequest()
    {
        return this.BadRequestResponse(detail: "This is a test bad request.");
    }
}

#endif