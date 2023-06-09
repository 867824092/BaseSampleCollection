using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        [HttpGet]
        public async Task Get() {
            await HttpContext.Response.WriteAsync(HttpContext.User.Identity.IsAuthenticated ? "当前已登录。" : "退出成功。");
        }

        [HttpGet("login")]
        public async Task Login() {
            var identity = new ClaimsIdentity(new Claim[]
            { new Claim(ClaimTypes.Role, "Admin") },CookieAuthenticationDefaults.AuthenticationScheme);
            var claimPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,claimPrincipal);
        }

        [HttpGet("logout")]
        public async Task Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        
    }
}
