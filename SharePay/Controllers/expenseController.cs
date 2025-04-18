using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharePay.Interfaces;
using SharePay.Models.ViewModels;

namespace SharePay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class expenseController : ControllerBase
    {
        private readonly IExpenseService service;
        public expenseController(IExpenseService service)
        {
            this.service = service;
        }

        [HttpPost("addsharedexp")]
        [Authorize]
        public async Task<IActionResult> AddSharedExpense(ExpenseVM exp)
        {
            if(exp != null)
            {
                var result = await service.AddExpense(exp);
                if (result.success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("No Body Found");
        }
    }
}
