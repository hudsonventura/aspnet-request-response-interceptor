using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RequestResponseInterceptor;

public abstract class AbstractInterceptor : IInterceptor
{
    public virtual void OnReceiveRequest(HttpRequest request)
    {
        throw new NotImplementedException();
    }

    public virtual void OnSendResponse(HttpResponse response, string body_string)
    {
        throw new NotImplementedException();
    }

    public IPAddress remoteIpAddress {get; protected set;}
    public void SetRemoteIP(IPAddress? remoteIpAddress)
    {
        this.remoteIpAddress = remoteIpAddress;
    }

    public string traceId {get; protected set;}
    public void SetTraceId(string traceId)
    {
        this.traceId = traceId;
    }
}
