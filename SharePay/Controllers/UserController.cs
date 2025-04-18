using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharePay.Interfaces;
using SharePay.Models;  

namespace SharePay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices services;
        public UserController(IUserServices services)
        {
            this.services = services;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(Users user)
        {
            var result = await services.RegisterUser(user);
            if (result.success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("userlogin")]
        public async Task<IActionResult> LoginUser(Users user)
        {
            var result = await services.Login(user);
            if(result.success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getallusers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await services.GetAllUsers();
            if (result.success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("searchuser")]
        [Authorize]
        public async Task<IActionResult> SearchUser(string prompt)
        {
            if (!string.IsNullOrEmpty(prompt))
            {
                var result = await services.SearchUser(prompt);
                if (result.success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Invalid Input");
        }
    }
}
