using System.Linq.Expressions;
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
    public IActionResult GetToken([FromBody] LoginDetails loginDetails)
    {
        // for now, just ensure the model is valid.
        // we can add proper user authentication later.
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        bool authenitcated = false;
        int userId = -1;
        
        // avoiding making a db call to validate the username and password
        if (loginDetails.UserName == "test" && loginDetails.Password == "password")
        {
            authenitcated = true;
            // in real code, the user id would be fetched from the database.
            userId = 1;
        }

        if (authenitcated)
        {
            var token = _tokenAuthenticator.GenerateToken(userId);

            return Json(new
            {
                result = HttpStatusCode.OK,
                token
            });
        }

        return Unauthorized();
    }
}