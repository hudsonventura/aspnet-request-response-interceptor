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
        this.interceptor = new InterceptorToTXTFile();
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
        Request request = await Request.Convert(context.Request);
        interceptor.OnReceiveRequest(request);
        context.Response.Headers.Add("traceId", traceId);





        //get the response body
        Stream originalbody = context.Response.Body;
        string response_body = "";
        try
        {
            using (var memstream = new MemoryStream())
            {
                context.Response.Body = memstream;

                
                try
                {
                    await _next(context);
    
                    memstream.Position = 0;
                    response_body = new StreamReader(memstream).ReadToEnd();
    
                    memstream.Position = 0;
                    await memstream.CopyToAsync(originalbody);

                    //gives the original body back
                    context.Response.Body = originalbody;
                    Response response = await Response.Convert(context.Response, response_body);

                    bool register = (context.Items.ContainsKey("IgnoreInterceptor") && bool.Parse(context.Items["IgnoreInterceptor"].ToString())) ? false : true;
                    if(register)
                        interceptor.OnSendResponse(response);
                }
                catch (System.Exception error)
                {
                    memstream.Position = 0;
                    response_body = new StreamReader(memstream).ReadToEnd();
    
                    memstream.Position = 0;
                    await memstream.CopyToAsync(originalbody);

                    //gives the original body back
                    context.Response.Body = originalbody;
                    context.Response.StatusCode = 500;

                    Response response = await Response.Convert(context.Response, error);

                    bool register = (context.Items.ContainsKey("IgnoreInterceptor") && bool.Parse(context.Items["IgnoreInterceptor"].ToString())) ? false : true;
                    if(register)
                        interceptor.OnSendResponse(response);

                    throw;
                }
            }
        }
        finally
        {
            
        }
        
    }

    
}


