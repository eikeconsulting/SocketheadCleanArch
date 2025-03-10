namespace SocketheadCleanArch.API.Models;

/// <summary>
/// The standard package for an API success response
/// If the API returns HTTP status code 200, then the call can assume it is a "success"
/// If the API returns another status code, it should also return a ProblemDetails;
/// See the ApiExceptionHandler for how that is populated
/// </summary>
public record ApiResponse<TData>(TData? Data, string? Message = null);
