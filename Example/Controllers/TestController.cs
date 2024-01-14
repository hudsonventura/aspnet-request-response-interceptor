using Microsoft.AspNetCore.Mvc;

namespace Tests.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{


    [HttpGet("/RequestEmptyBody_ReponseEmptyBody")]
    public IActionResult RequestEmptyBody_ReponseEmptyBody()
    {
        return Ok();
    }

    [HttpPost("/RequestEmptyBody_ReponseWithSomeBody")]
    public IActionResult RequestEmptyBody_ReponseWithSomeBody()
    {
        return Ok("Ok");
    }

    [HttpPost("/RequestWithSomeBody_ReponseEmptyBody")]
    public IActionResult RequestWithSomeBody_ReponseEmptyBody([FromBody] string teste)
    {
        return Ok();
    }

    [HttpPost("/RequestWithSomeBody_ReponseWithSomeBody")]
    public IActionResult RequestWithSomeBody_ReponseWithSomeBody([FromBody] string teste)
    {
        return Ok(teste);
    }
}
