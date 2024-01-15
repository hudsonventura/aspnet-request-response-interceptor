# RequestResponseInterceptor
Nuget middleware for ASP.NET Core package to intercept request before send do controller and response before send to client. This is useful to automatically log requests and responses, for example. An implementations was made and it can log method, url, endpoint, header, query, body, status code, both the request and the response.

<div align="center">

![Teste](https://raw.githubusercontent.com/hudsonventura/aspnet-request-response-logger/main/assets/icon.png)

</div>

## File Program.cs (net6.0)

```C#
using RequestResponseInterceptor;
using RequestResponseInterceptor.Implementations; //If you want to use my implmentation. If you will create yours, you can remove this.

```


Using my class Interceptor (you can create your own. See on section [How can I create my own Interceptor](#How-can-I-create-my-own-Interceptor))

```C#
InterceptorOptions options = new InterceptorOptions(){
    //If you are using docker container logs, leave it enabled. It is going to agrupate whole request and reponse line and will write to logs in the end
    WriteRequestAndResponseTogetherInTheEnd = true, //default true

    //If you are using docker container logs, leave it enabled. It will be easier to search by 'traceId'.
    //Look the image below. This is the data highlighted in yellow
    WriteTraceIDBeforEachLine = true,  //default true
};
IInterceptor interceptor = new Interceptor(options);
app.UseInterceptor(interceptor);

```


or this to default values of InterceptorOptions
```C#
IInterceptor interceptor = new Interceptor();
app.UseInterceptor(interceptor);
```



or just
```C#
app.UseInterceptor();
```




### What it will do?

It will write to **console** and a **log file** (dir logs) all data of request and response, like this:


![Print](https://raw.githubusercontent.com/hudsonventura/aspnet-request-response-interceptor/main/assets/print1.png)



But you don't need to use my implementation. So you can create your own classe to log by you way.  
In this case, just implements the interface `RequestResponseInterceptor.IInterceptor`.  

``` C#
public interface IInterceptor
{
    // On complete receipt of the request, before starting processing by your controller
    void OnReceiveRequest(HttpRequest request);

    // After processing your controller, before returning to client
    void OnSendResponse(HttpResponse response, string body_string);

    // Set the IP
    void SetRemoteIP(IPAddress? remoteIpAddress);

    // Set TraceId for each request
    void SetTraceId(string traceId);
}
```


An a example of my implementation is at file `src.Implementations.Interceptor.cs`. You can use it to get inspired and create your own implementation, saving data to disk, database, or calling other services.  

# How can I create my own Interceptor?

First, create a class and it implement the `RequestResponseInterceptor.IInterceptor` interface, like below.

```C#
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
```



Now you have to implement each function. If you don't want to use `SetTraceId` or `SetRemoteIP`, just add return like below. These are not necessary.

```C#
    public void SetRemoteIP(IPAddress? remoteIpAddress)
    {
        return;
    }

    public void SetTraceId(string traceId)
    {
        return;
    }
```

Finally you have to inject your class no `Program.cs`.
```C#
YourClass interceptor = new YourClass();
app.UseInterceptor(interceptor);
```

This plugin will call your funcion `OnReceiveRequest` befor call your controller, and after the processing before send data to client (requester), it will call your funciont `OnSendResponse`.  
The funcionts `SetRemoteIP` and `SetTraceId` will call before `OnReceiveRequest`.