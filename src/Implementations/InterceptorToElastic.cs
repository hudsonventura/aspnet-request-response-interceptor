using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace RequestResponseInterceptor.Implementations;

public class InterceptorToElastic : AbstractInterceptor, IInterceptor
{
    HttpClient client;
    string host;
    string index;
    InterceptorOptions options;

    public InterceptorToElastic(string host)
    {
        this.host = host;
        this.index = "RequestResponseInterceptor";
        client = new HttpClient();
    }

    public InterceptorToElastic(string host, string index, string authorization = null, InterceptorOptions options = null)
    {
        this.host = host;
        this.index = index;
        client = new HttpClient();
        if(authorization is not null){
            client.DefaultRequestHeaders.Add("Authorization", authorization);
        }
        this.options = options;
    }




    Request request;
    public override async void OnReceiveRequest(Request request)
    {
        this.request = request;
    }

    public override async void OnSendResponse(Response response)
    {
        if(options.LogGetRequest == false && request.Method == "GET"){
            return;
        }
        
        try
        {
            string apiUrl = $"{host}/{index}/_doc";

            string levelString = "Info";
            switch (response.StatusCode.ToString().Substring(0, 1))
            {
                case "2": levelString = "Info";
                break;

                case "3": levelString = "Warning";
                break;

                case "4": levelString = "Warning";
                break;

                case "5": levelString = "Error";
                break;

                default: levelString = "Error";
                break;
            }

            var objetoParaEnviar = new
            {
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                message = $"Request {traceId} from {remoteIpAddress}",
                level = levelString,
                Request = request,
                Response = (response.Exception is not null) ? null : response,
                Exception = (response.Exception is not null) ? response.Exception : null,
            };



            string jsonBody = JsonConvert.SerializeObject(objetoParaEnviar);

            // Configurar o conteúdo da solicitação
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Fazer uma solicitação POST
            HttpResponseMessage responseElastic = await client.PostAsync(apiUrl, content);

        }
        catch (Exception ex)
        {
            //Console.WriteLine($"Erro ao fazer a solicitação: {ex.Message}");
        }
    }

}
