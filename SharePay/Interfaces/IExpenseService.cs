using SharePay.Models.ViewModels;
using SharePay.Models;

namespace SharePay.Interfaces
{
    public interface IExpenseService
    {
        Task<ApiResponse<bool>> AddExpense(ExpenseVM expense);
        Task<ApiResponse<IEnumerable<Expenses>>> GetUserPaidExpenses();
        Task<ApiResponse<IEnumerable<UserExpenseVM>>> GetExpenseDetails(int expId);
    }
}
