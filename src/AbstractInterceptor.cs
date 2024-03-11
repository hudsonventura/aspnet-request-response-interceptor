using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RequestResponseInterceptor;

public abstract class AbstractInterceptor : IInterceptor
{
    protected AbstractInterceptor()
    {
    }

    protected InterceptorOptions options;
    protected AbstractInterceptor(InterceptorOptions options)
    {
        this.options = options;
    }

    protected Request request;
    public virtual void OnReceiveRequest(Request request)
    {
        throw new NotImplementedException("You have to implement your own interceptor. See doc at https://github.com/hudsonventura/aspnet-request-response-interceptor");
    }

    public virtual void OnSendResponse(Response response)
    {
        throw new NotImplementedException("You have to implement your own interceptor. See doc at https://github.com/hudsonventura/aspnet-request-response-interceptor");
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
