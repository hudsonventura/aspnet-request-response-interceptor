using System.Net;
using RequestResponseInterceptor;

namespace YourNameSpace;

public class YourClass : IInterceptor
{
    public void OnReceiveRequest(HttpRequest request)
    {
        throw new NotImplementedException();
    }

    public void OnSendResponse(HttpResponse response, string body_string)
    {
        throw new NotImplementedException();
    }

    public void SetRemoteIP(IPAddress? remoteIpAddress)
    {
        throw new NotImplementedException();
    }

    public void SetTraceId(string traceId)
    {
        throw new NotImplementedException();
    }
}
