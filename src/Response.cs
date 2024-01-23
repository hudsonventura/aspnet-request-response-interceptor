using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace RequestResponseInterceptor;

public class Response
{
    public int StatusCode { get; private set; }
    public string ReasonPhrase { get; private set; }
    public Dictionary<string, string> Headers { get; private set; }
    public string Body { get; private set; }

    internal static async Task<Response> Convert(HttpResponse response, string body)
    {
        string bodyLines = (body.Length > 0) ? body : "null";
        if (response.Headers.ContainsKey("Content-Type") && response.Headers["Content-Type"].ToString().Contains("json"))
        { //formata o json para ser identado
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented // Define o formato como indentado (beautify)
            };

            // Serializa o objeto de volta para uma string JSON formatada
            dynamic objetoDynamic = JsonConvert.DeserializeObject(body);
            bodyLines = JsonConvert.SerializeObject(objetoDynamic, settings);

        }
        bodyLines = string.Join(Environment.NewLine, bodyLines.Split('\n').Select(line => line));


        return new Response(){
            StatusCode = response.StatusCode,
            ReasonPhrase = ((HttpStatusCode)response.StatusCode).ToString(),
            Headers = response.Headers.ToDictionary(q => q.Key, q => q.Value.ToString()),
            Body = bodyLines,
        };

        
    }
}
