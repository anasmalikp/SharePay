using SharePay.Models.ViewModels;
using SharePay.Models;

namespace SharePay.Interfaces
{
    public interface IExpenseService
    {
        Task<ApiResponse<bool>> AddExpense(ExpenseVM expense);
    }
}
