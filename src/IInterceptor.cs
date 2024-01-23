using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RequestResponseInterceptor;

public interface IInterceptor
{
    // On complete receipt of the request, before starting processing by your controller
    void OnReceiveRequest(Request request);

    // After processing your controller, before returning to client
    void OnSendResponse(Response response);

    // Set the IP
    void SetRemoteIP(IPAddress? remoteIpAddress);

    // Set TraceId for each request
    void SetTraceId(string traceId);
}
