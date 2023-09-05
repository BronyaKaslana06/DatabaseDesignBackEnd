using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi;

namespace webapi.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly JwtHelper _jwtHelper;

    public TestController(JwtHelper jwtHelper)
    {
        _jwtHelper = jwtHelper;
    }
    [HttpGet]
    public ActionResult<string> value()
    {
        return "Without Authorize";
    }
    [Authorize]
    [HttpGet]
    public ActionResult<string> GetTest()
    {
        return "Test Authorize";
    }
}
