using System.Net;
using System.Text;

namespace Tests_Integration;

public class UnitTest1
{
    HttpClient client = new Host().CreateClient();

    public UnitTest1() {
        
    }

    private StringContent GenerateContent() {
        string requestBody = "\"Some body\"";
       return new StringContent(requestBody, Encoding.UTF8, "application/json");
    }
  

    [Fact]
    public async Task RequestEmptyBody_ResponseEmptyBody() 
    {
        var result = await client.GetAsync($"/RequestEmptyBody_ResponseEmptyBody");
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task RequestEmptyBody_ResponseWithSomeBody() 
    {
        var result = await client.PostAsync("/RequestEmptyBody_ResponseWithSomeBody", GenerateContent());
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }


    [Fact]
    public async Task RequestWithSomeBody_ResponseEmptyBody() 
    {
        var result = await client.PostAsync($"/RequestWithSomeBody_ResponseEmptyBody", GenerateContent());
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task RequestWithSomeBody_ResponseWithSomeBody() 
    {
        var result = await client.PostAsync($"/RequestWithSomeBody_ResponseWithSomeBody", GenerateContent());
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task RequestWithQuery() 
    {
        var queryParams = new { parametro1 = "valor1", parametro2 = "valor2" };
        string url = $"/RequestEmptyBody_ResponseEmptyBody?parametro1={queryParams.parametro1}&parametro2={queryParams.parametro2}";
        var result = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task Test500() 
    {
        var result = await client.PostAsync($"/Test500", GenerateContent());
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
    }
}