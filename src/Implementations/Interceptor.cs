using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RequestReponseInterceptor;


namespace RequestReponseInterceptor.Implementations;

public class Interceptor : IInterceptor
{

    DateTime startTime;
    DateTime endTime;

    public Interceptor()
    {
        //get ths options with the default values
        this.options = new InterceptorOptions();
    }

    //to write the log on a file, database or anywhere
    LogWriter writer = new LogWriter();

    InterceptorOptions options;
    public Interceptor(InterceptorOptions options){
        this.options = options;
    }


    HttpRequest request;
    
    bool showTracerId = true;
    string traceId;
    public void SetTraceId(string traceId)
    {
        this.traceId = traceId;
    }

    IPAddress remoteIpAddress;
    public void SetRemoteIP(IPAddress? remoteIpAddress)
    {
        this.remoteIpAddress = remoteIpAddress;
    }

    public async void OnReceiveRequest(HttpRequest request)
    {
        showTracerId = options.WriteTraceIDBeforEachLine;
        startTime = DateTime.Now;

        if(options.WriteRequestAndResponseTogetherInTheEnd == false){
            await WriteOutRequest(request);
            return;
        }

        this.request = request;
    }

    private async Task WriteOutRequest(HttpRequest request)
    {
        var trace_id = (showTracerId == false) ? "": $"{traceId} - ";

        //add query params
        string query = "";
        if(request.Query.Count() > 0){
            query = "?" + string.Join("&", request.Query.Select(q => $"{q.Key}={q.Value}"));
        }


        var body = await ReadRequestBody(request);
        var bodyLines = string.Join(Environment.NewLine, body.Split('\n').Select(line => $"{trace_id}{line}"));

        var endpointLine = $"{trace_id}{request.Method} {request.Host}{request.Path}{query}";

        var headersLines = string.Join(Environment.NewLine, request.Headers.Select(header => $"{trace_id}{header.Key}: {header.Value}"));
        
        string log = $"{endpointLine}{Environment.NewLine}{headersLines}{Environment.NewLine}{trace_id}{Environment.NewLine}{bodyLines}{Environment.NewLine}";

        string separator = $"{Environment.NewLine}{trace_id}REQUEST at {startTime.ToString("yyyy/MM/dd HH:mm:ss:fff")} from {remoteIpAddress} ---------------------------------------------------------------------------{Environment.NewLine}";


        Console.WriteLine(separator);
        Console.WriteLine(log);

        writer.WriteLog(separator);
        writer.WriteLog(log);
    }









    public async void OnSendResponse(HttpResponse response, string body_string)
    {
        endTime = DateTime.Now;
        showTracerId = options.WriteTraceIDBeforEachLine;
        

        if(options.WriteRequestAndResponseTogetherInTheEnd == true){
            await WriteOutRequest(request);
        }

        
        WriteOutResponse(response, body_string);
    }

    private void WriteOutResponse(HttpResponse response, string original_response_body)
    {
        var totalTime = endTime - startTime;
        var trace_id = (showTracerId == false) ? "": $"{traceId} - ";

        //get and format the body
        string bodyLines = (original_response_body.Length > 0) ? original_response_body : "null";
        if (response.Headers.ContainsKey("Content-Type") && response.Headers["Content-Type"].ToString().Contains("json"))
        { //formata o json para ser identado
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented // Define o formato como indentado (beautify)
            };

            // Serializa o objeto de volta para uma string JSON formatada
            dynamic objetoDynamic = JsonConvert.DeserializeObject(original_response_body);
            bodyLines = JsonConvert.SerializeObject(objetoDynamic, settings);

        }
        bodyLines = string.Join(Environment.NewLine, bodyLines.Split('\n').Select(line => $"{trace_id}{line}"));

        string separator = $"{Environment.NewLine}{trace_id}RESPONSE at {endTime.ToString("yyyy/MM/dd HH:mm:ss:fff")} to {remoteIpAddress}, total time {totalTime.Seconds}s -------------------------------------------------------------{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}";
        var statusCodeLine = $"{trace_id}StatusCode: {response.StatusCode} - {(HttpStatusCode)response.StatusCode}";
        var headersLines = string.Join(Environment.NewLine, response.Headers.Select(header => $"{trace_id}{header.Key}: {header.Value}"));
        string log = $"{statusCodeLine}{Environment.NewLine}{headersLines}{Environment.NewLine}{trace_id}{Environment.NewLine}{bodyLines}";


        Console.WriteLine(log);
        Console.WriteLine(separator);

        writer.WriteLog(log);
        writer.WriteLog(separator);
    }










    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        // Configure o corpo da requisição para permitir a leitura posterior
        request.EnableBuffering();

        // Lê o corpo da requisição como uma string sem consumi-lo
        using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true))
        {
            string body = await reader.ReadToEndAsync();

            // Volta ao início do fluxo para que o corpo possa ser lido novamente posteriormente
            request.Body.Seek(0, SeekOrigin.Begin);

            return (body.Length > 0) ? body : "null";
        }
    }

    
}
