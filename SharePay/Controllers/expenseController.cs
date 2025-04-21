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

        [HttpGet("getuserpaidexp")]
        [Authorize]
        public async Task<IActionResult> GetUserPaidExp()
        {
            var result = await service.GetUserPaidExpenses();
            if (result.success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getexpdetails")]
        [Authorize]
        public async Task<IActionResult> GetExpDetails(int expId)
        {
            var result = await service.GetExpenseDetails(expId);
            if (result.success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getallpayables")]
        [Authorize]
        public async Task<IActionResult> GetPayables()
        {
            var response = await service.GetAllPayableExpenses();
            if (response.success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("postpayment")]
        [Authorize]
        public async Task<IActionResult> PostPayment(int amount, int expId)
        {
            if(amount <= 0 ||  expId <= 0)
            {
                return BadRequest("Invalid Inputs");
            }

            var result = await service.ExpPayment(amount, expId);
            if (result.success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
