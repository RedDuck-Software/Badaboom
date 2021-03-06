using BackendCore.Models.Request;
using BackendCore.Services;
using Badaboom.Backend.Controllers;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("AllowAll")]
    public class AuthController : BaseController
    {
        private readonly ILogger<AuthController> _logger;
        
        private readonly IUserService _userService;

        public AuthController(ILogger<AuthController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest model)
        {
            var response = await _userService.Register(model);

            if (response == null) return new BadRequestObjectResult("User with this address is already exist");

            return new OkObjectResult(response);
        } 

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = await _userService.Authenticate(model, GetIpAddress());

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var refreshToken = Request.Cookies["refreshToken"] ?? request.Token;

            var response = await _userService.RefreshToken(refreshToken);

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            return Ok(response);
        }

        [HttpPost("revokeToken")]
        [Badaboom.Backend.Attributes.Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var response = await _userService.RevokeToken(token);

            if (!response)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }


        [HttpGet("/api/auth/user/{address}")]
        public async Task<IActionResult> GetUserByAddress(string address)
        {
            var user = await _userService.GetUserByAddress(address);
            
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("userAddress")]
        [Badaboom.Backend.Attributes.Authorize]
        public IActionResult GetUserAddress()
        {
            return Ok(new { address = CurrentUser.Address });
        }


        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
