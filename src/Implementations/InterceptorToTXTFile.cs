using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RequestResponseInterceptor;


namespace RequestResponseInterceptor.Implementations;

public class InterceptorToTXTFile : AbstractInterceptor, IInterceptor
{

    DateTime startTime;
    DateTime endTime;

    public InterceptorToTXTFile()
    {
        //get ths options with the default values
        this.options = new InterceptorOptions();
    }

    InterceptorOptions options;
    public InterceptorToTXTFile(InterceptorOptions options){
        this.options = options;
    }


    Request request;
    
    bool showTracerId = true;


    public async void OnReceiveRequest(Request request)
    {
        showTracerId = options.WriteTraceIDBeforEachLine;
        startTime = DateTime.Now;

        if(options.WriteRequestAndResponseTogetherInTheEnd == false){
            await WriteOutRequest(request);
            return;
        }

        this.request = request;
    }

    private async Task WriteOutRequest(Request request)
    {
        var trace_id = (showTracerId == false) ? "": $"{traceId} - ";

        //add query params
        string query = "";
        if(request.Query.Count() > 0){
            query = "?" + string.Join("&", request.Query.Select(q => $"{q.Key}={q.Value}"));
        }


        var bodyLines = string.Join(Environment.NewLine, request.Body.Split('\n').Select(line => $"{trace_id}{line}"));

        var endpointLine = $"{trace_id}{request.Method} {request.Host}{request.Path}{query}";

        var headersLines = string.Join(Environment.NewLine, request.Headers.Select(header => $"{trace_id}{header.Key}: {header.Value}"));
        
        string log = $"{endpointLine}{Environment.NewLine}{headersLines}{Environment.NewLine}{trace_id}{Environment.NewLine}{bodyLines}{Environment.NewLine}";

        string separator = $"{Environment.NewLine}{trace_id}REQUEST at {startTime.ToString("yyyy/MM/dd HH:mm:ss:fff")} from {remoteIpAddress} ---------------------------------------------------------------------------{Environment.NewLine}";


        Console.WriteLine(separator);
        Console.WriteLine(log);

        WriteLog(separator);
        WriteLog(log);
    }









    public async void OnSendResponse(Response response)
    {
        endTime = DateTime.Now;
        showTracerId = options.WriteTraceIDBeforEachLine;
        

        if(options.WriteRequestAndResponseTogetherInTheEnd == true){
            await WriteOutRequest(request);
        }

        
        WriteOutResponse(response);
    }

    private void WriteOutResponse(Response response)
    {
        var totalTime = endTime - startTime;
        var trace_id = (showTracerId == false) ? "": $"{traceId} - ";


        var bodyLines = string.Join(Environment.NewLine, response.Body.Split('\n').Select(line => $"{trace_id}{line}"));

        string separator = $"{Environment.NewLine}{trace_id}RESPONSE at {endTime.ToString("yyyy/MM/dd HH:mm:ss:fff")} to {remoteIpAddress}, total time {totalTime.Seconds}s -------------------------------------------------------------{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}";
        var statusCodeLine = $"{trace_id}StatusCode: {response.StatusCode} - {(HttpStatusCode)response.StatusCode}";
        var headersLines = string.Join(Environment.NewLine, response.Headers.Select(header => $"{trace_id}{header.Key}: {header.Value}"));
        string log = $"{statusCodeLine}{Environment.NewLine}{headersLines}{Environment.NewLine}{trace_id}{Environment.NewLine}{bodyLines}";


        Console.WriteLine(log);
        Console.WriteLine(separator);

        WriteLog(log);
        WriteLog(separator);
    }




    private void WriteLog(string logMessage)
    {
        // Obtenha o diretório corrente
        string currentDirectory = Directory.GetCurrentDirectory();

        // Crie um subdiretório para o mês atual
        string monthDirectory = Path.Combine(currentDirectory, "logs");

        // Verifique se o subdiretório já existe, se não, crie
        if (!Directory.Exists(monthDirectory))
        {
            Directory.CreateDirectory(monthDirectory);
        }

        // Crie um nome de arquivo com base na data e hora atual
        string fileName = Path.Combine(monthDirectory, $"{DateTime.Now.ToString("yyyy-MM-dd")}.log");

        // Escreva o log no arquivo, fazendo um append ao conteúdo existente
        File.AppendAllText(fileName, logMessage);
    }





    
}
