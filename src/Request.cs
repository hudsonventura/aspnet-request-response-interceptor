using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace RequestResponseInterceptor;

public class Request
{
    public string Method { get; private set; }
    public Dictionary<string, string> Headers { get; private set; }
    public string Host { get; private set; }
    public string Path { get; private set; }
    public Dictionary<string, string> Query { get; private set; }
    public string CompleteURL { get; private set; }
    public string Body { get; private set; }

    internal static async Task<Request> Convert(HttpRequest request)
    {
        //Query params
        string query = "";
        if(request.Query.Count() > 0){
            query = "?" + string.Join("&", request.Query.Select(q => $"{q.Key}={q.Value}"));
        }

        var completeURL = $"{request.Host}{request.Path}{query}";


        return new Request(){
            Method = request.Method,
            Headers = request.Headers.ToDictionary(q => q.Key, q => q.Value.ToString()),
            Host = request.Host.ToString(),
            Path = request.Path.ToString(),
            Query = request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()),
            CompleteURL = completeURL,
            Body = await ReadBody(request),
        };

        
    }

    private async static Task<string> ReadBody(HttpRequest request)
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
