# RequestResponseInterceptor


<div align="center">

![Teste](https://raw.githubusercontent.com/hudsonventura/aspnet-request-response-logger/main/assets/icon.png)

</div>

Nuget middleware for ASP.NET Core package to intercept request before send do controller and response before send to client. This is useful to automatically log requests and responses, for example. Two implementations were made, one to save request and response in a TXT file and another to send the data to ElasticSearch / Elastic Stack. Additionally, you can create `your own implementation`.  

## Get Starting

Install the lib
```bash
dotnet add package RequestResponseInterceptor
```

In the ile Program.cs (net6.0) put this for all use cases.

```C#
using RequestResponseInterceptor;
...
app.UseInterceptor();
```  

### How to log request and response to TXT file?

```C#
using RequestResponseInterceptor.Implementations;
...
InterceptorOptions options = new InterceptorOptions(){
    //If you are using docker container logs, leave it enabled. It is going to agrupate whole request and reponse line and will write to logs in the end
    WriteRequestAndResponseTogetherInTheEnd = true, //default true

    //If you are using docker container logs, leave it enabled. It will be easier to search by 'traceId'.
    //Look the image below. This is the data highlighted in yellow
    WriteTraceIDBeforEachLine = true,  //default true
};
IInterceptor interceptor = new InterceptorToTXTFile(options);
app.UseInterceptor(interceptor);

```


or just this to default values of InterceptorOptions
```C#
using RequestResponseInterceptor.Implementations;
...
IInterceptor interceptor = new InterceptorToTXTFile();
app.UseInterceptor(interceptor);
```

It will write to **console** and a **log file** (dir logs) all data of request and response, like this:


![Print](https://raw.githubusercontent.com/hudsonventura/aspnet-request-response-interceptor/main/assets/print1.png)


But you don't need to use my class Interceptor to write to file, you can create your own. See on section [How can I create my own Interceptor](#How-can-I-create-my-own-Interceptor))

---

### How to log request and response to ElasticSearch / Elastic Stack?

```C#
using RequestResponseInterceptor.Implementations;
...
IInterceptor interceptor = new InterceptorToElastic("http://localhost:9200", "YOUR_ELASTIC_INDEX");
app.UseInterceptor(interceptor);
```

After add your index, go to Kibana > Discovery.  

![Print](https://raw.githubusercontent.com/hudsonventura/aspnet-request-response-logger/main/assets/kibana_print.png)  

Change to your index.  

![Print](https://raw.githubusercontent.com/hudsonventura/aspnet-request-response-logger/main/assets/kibana_print2.png)

You will find your request and response  

![Print](https://raw.githubusercontent.com/hudsonventura/aspnet-request-response-logger/main/assets/log_elastic.png)


---

### How can I create my own Interceptor? Building your own logger.

You don't need to use my implementations. So you can create your own classe to log by you way.  
In this case, create a class and it implement the `IInterceptor` interface and inherit the class `AbstractInterceptor`, like below.




Now you have to implement the functions `OnReceiveRequest` and `OnSendResponse`, as below.  
As a gift you will receive the variables `remoteIpAddress` (the ip of client requester) and `TraceId` (the request id that will be in the request and response header)

``` C#
using RequestResponseInterceptor;

namespace Example;

public class MyInterceptor : AbstractInterceptor, IInterceptor
{
    public override void OnReceiveRequest(Request request)
    {
        throw new NotImplementedException();
    }

    public override void OnSendResponse(Response response)
    {
        throw new NotImplementedException();
    }
}

```

Finally you have to inject your class in the `Program.cs`.
```C#
YourClass interceptor = new YourClass();
app.UseInterceptor(interceptor);
```

This lib will call your funcion `OnReceiveRequest` befor call your controller, and after the processing, before send data to client (requester), it will call the function `OnSendResponse`.  
The funcionts `SetRemoteIP` and `SetTraceId` will call before `OnReceiveRequest`.