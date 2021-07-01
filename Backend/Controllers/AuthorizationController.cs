using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("~/testGet")]
        public string TestGet()
        {
            return "test get anonymous!";
        }

        [Authorize]
        [HttpGet("~/testGetAuthorized")]
        public string TestGetAuthorized()
        {
            return "test get authtorized!";
        }
    }
}
