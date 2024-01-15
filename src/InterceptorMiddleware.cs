using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Diagnostics;
using Newtonsoft.Json;

using RequestResponseInterceptor;
using RequestResponseInterceptor.Implementations;



namespace RequestResponseInterceptor;


public static class InterceptorMiddlewareExtensions
{
     public static IApplicationBuilder UseInterceptor(this IApplicationBuilder builder,  IInterceptor intercept)
    {
        return builder.UseMiddleware<InterceptorMiddleware>(intercept);
    }
}


public class InterceptorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IInterceptor? interceptor;


    public InterceptorMiddleware(RequestDelegate next)
    {
        this.interceptor = new Interceptor();
        this._next = next;
    }

    public InterceptorMiddleware(RequestDelegate next, IInterceptor? interceptor)
    {
        this.interceptor = interceptor;
        this._next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string traceId = Activity.Current?.Id ?? context?.TraceIdentifier;
        interceptor.SetTraceId(traceId);
        interceptor.SetRemoteIP(context.Connection.RemoteIpAddress);
        

        context.Request.Headers.Add("traceId", traceId);
        interceptor.OnReceiveRequest(context.Request);
        context.Response.Headers.Add("traceId", traceId);





        //get the response body
        Stream originalbody = context.Response.Body;
        string response_body = "";
        try
        {
            using (var memstream = new MemoryStream())
            {
                context.Response.Body = memstream;

                await _next(context);

                memstream.Position = 0;
                response_body = new StreamReader(memstream).ReadToEnd();

                memstream.Position = 0;
                await memstream.CopyToAsync(originalbody);
            }
        }
        finally
        {
            //gives the original body back
            context.Response.Body = originalbody;
        }
        interceptor.OnSendResponse(context.Response, response_body);
    }
}


