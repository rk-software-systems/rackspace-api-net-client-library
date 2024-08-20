using Microsoft.Extensions.Logging;

namespace RKSoftware.Rackspace.ApiClient;

internal sealed class RackspaceLoggingConstants
{
    internal const int RequestError             = 1111111;
    internal const int AccessTokenNotFoundError = 1111112;
    internal const int EndpointsNotFoundError   = 1111113;

    internal const int CdnEndpointUrlNotFoundError  = 1111114;
    internal const int EndpointUrlNotFoundError     = 1111115;

    public static readonly Action<ILogger, string, string, Exception?> LogRequestError = LoggerMessage.Define<string, string>(
        LogLevel.Error,
        RequestError,
        "Request '{Request}' is not success. Response: '{Response}'.");

    public static readonly Action<ILogger, Exception?> LogAccessTokenNotFoundError = LoggerMessage.Define(
        LogLevel.Error,
        AccessTokenNotFoundError,
        "Access Token is not found in the response.");

    public static readonly Action<ILogger, Exception?> LogEndpointsNotFoundError = LoggerMessage.Define(
        LogLevel.Error,
        EndpointsNotFoundError,
        "Endpoints are not found in the response.");

    public static readonly Action<ILogger, string, string, Exception?> LogCdnEndpointUrlNotFoundError = LoggerMessage.Define<string, string>(
        LogLevel.Error,
        CdnEndpointUrlNotFoundError,
        "CDN endpoint is not found for region '{Region}'. Method: '{Method}'.");

    public static readonly Action<ILogger, string, string, Exception?> LogEndpointUrlNotFoundError = LoggerMessage.Define<string, string>(
        LogLevel.Error,
        RackspaceLoggingConstants.EndpointUrlNotFoundError,
        "Endpoint is not found for region '{Region}'. Method: '{Method}'.");
}
