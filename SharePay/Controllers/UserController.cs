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
    }
}
