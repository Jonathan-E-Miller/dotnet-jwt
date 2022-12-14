using System.Net;
using Microsoft.AspNetCore.Mvc;
using TokenAuthenticator;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly ITokenAuthenticator _tokenAuthenticator;
    
    public AuthController(ITokenAuthenticator tokenAuthenticator)
    {
        _tokenAuthenticator = tokenAuthenticator;
    }
    
    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetToken([FromBody]LoginDetails loginDetails)
    {
        // for now, just ensure the model is valid.
        // we can add proper user authentication later.
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = 1;

        var token = _tokenAuthenticator.GeneratorToken(userId);

        return Json(new
        {
            result = HttpStatusCode.OK,
            token
        });
    }
}