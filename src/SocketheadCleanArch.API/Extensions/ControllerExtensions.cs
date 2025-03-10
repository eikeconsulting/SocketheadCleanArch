using Microsoft.AspNetCore.Mvc;
using SocketheadCleanArch.API.Models;

namespace SocketheadCleanArch.API.Extensions;

public static class ControllerExtensions
{
    public static ActionResult<ApiResponse<TData>> OkResponse<TData>(
        this ControllerBase controller, 
        TData? data = default,
        string? message = null) =>
        controller.Ok(new ApiResponse<TData>(data, message));

    public static ObjectResult ProblemResponse(
        this ControllerBase controller, 
        string? title = null, 
        string? detail = null, 
        string? type = null, 
        int? statusCode = null) =>
        controller.Problem(
            title: title ?? "Bad Request",
            detail: detail ?? "Something went wrong while processing your request.",
            type: type ?? "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            statusCode: statusCode ?? StatusCodes.Status400BadRequest);

    public static ObjectResult BadRequestResponse(this ControllerBase controller, string detail) => 
        controller.ProblemResponse(
            title: "Bad Request",
            detail: detail,
            statusCode: StatusCodes.Status400BadRequest);

    public static ObjectResult NotAuthorizedResponse(this ControllerBase controller, string detail) =>
        controller.ProblemResponse(
            title: "Not Authorized",
            detail: detail,
            statusCode: StatusCodes.Status401Unauthorized);
    
    public static ObjectResult NotFoundResponse(this ControllerBase controller) =>
        controller.ProblemResponse(
            title: "Resource Not Found",
            statusCode: StatusCodes.Status404NotFound);
}