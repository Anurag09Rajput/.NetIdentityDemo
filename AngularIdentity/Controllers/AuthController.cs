using Application.AngularIdentity.Concerns;
using Application.AngularIdentity.Contracts;
using Domain.AngularIdentity.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace AngularIdentity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
       
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto model)
        {
            if (model == null)
            {
                return BadRequest("Please fill the details.!");
            }

            Response result = await this._authService.LoginUser(model);
            return result.IsSuccess ? Ok(result) : BadRequest(result);

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserForRegistrationDto model)
        {
            if(model  == null)
            {
                return BadRequest("Please fill the details.!");
            }

            Response result = await this._authService.RegisterUser(model);
            
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
