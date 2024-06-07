using Tests;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{


    [HttpGet("/RequestEmptyBody_ResponseEmptyBody")]
    public IActionResult RequestEmptyBody_ResponseEmptyBody()
    {
        HttpContext.Items["IgnoreInterceptor"] = false;
        return Ok();
    }

    [HttpPost("/RequestEmptyBody_ResponseWithSomeBody")]
    public IActionResult RequestEmptyBody_ResponseWithSomeBody()
    {
        HttpContext.Items["IgnoreInterceptor"] = true;
        return Ok("Ok");
    }

    [HttpPost("/RequestWithSomeBody_ResponseEmptyBody")]
    public IActionResult RequestWithSomeBody_ResponseEmptyBody([FromBody] string teste)
    {
        return Ok();
    }

    [HttpPost("/RequestWithSomeBody_ResponseWithSomeBody")]
    public IActionResult RequestWithSomeBody_ResponseWithSomeBody([FromBody] ObjTest teste)
    {
        return Ok(teste);
    }

    [HttpPost("/Test500")]
    public IActionResult Test500()
    {
        throw new Exception("Wrong error");
    }
}
