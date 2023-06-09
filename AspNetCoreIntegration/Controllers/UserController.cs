using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        //[Authorize("CookieDefault")]
        [Authorize]
        [HttpGet]
        public async Task Get() {
            await HttpContext.Response.WriteAsync($"当前用户身份：{HttpContext.User.Claims.FirstOrDefault(u=>u.Type== ClaimTypes.Role)!.Value}");
        }
    }
}
