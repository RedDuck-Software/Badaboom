using Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace Badaboom.Backend.Controllers
{
    public class BaseController : ControllerBase
    {
        public User CurrentUser => (User)HttpContext.Items["User"];
    }
}
