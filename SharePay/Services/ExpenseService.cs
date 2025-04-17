using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using SharePay.Models;
using SharePay.Models.ViewModels;

namespace SharePay.Services
{
    public class ExpenseService
    {
        private readonly ILogger<ExpenseService> logger;
        private readonly IDbConnection connection;
        private readonly UserCreds creds;

        public ExpenseService(ILogger<ExpenseService> logger, IConfiguration config, UserCreds creds)
        {
            this.logger = logger;
            this.creds = creds;
            connection = new MySqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        public async Task<ApiResponse<bool>> AddExpense(ExpenseVM expense)
        {
            try
            {
                Expenses exp = new Expenses()
                {
                    amount = expense.amount,
                    isSettled = false,
                    name = expense.name,
                    Note = expense.Note
                };

                int expId = await connection.ExecuteScalarAsync<int>("INSERT INTO expenses (amount, isSettled, Note, name, paidBy) VALUES (@amount, @isSettled, @Note, @name, @paidBy)", new { amount = expense.amount, isSettled = false, Note = expense.Note, name = expense.name, paidBy=creds.UserId });
                if(expId == 0)
                {
                    return new ApiResponse<bool>
                    {
                        success = false,
                        status = 400,
                        message = "Failed to add the expense",
                        data = false
                    };
                }
                expense.users.Add(creds.UserId);

                foreach(var usr in expense.users)
                {
                    await connection.ExecuteAsync("INSERT INTO userexpenses (expId, paidAmt, userAmount, userId) VALUES (@expId, @paidAmt, @userAmount, @userId)", new { expId = expId, paidAmt = 0, userAmount = expense.amount/expense.users.Count, userId = usr});
                }

                return new ApiResponse<bool>
                {
                    success = true,
                    status = 201,
                    message = "Expense Created and Shared",
                    data = true
                };

            } catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new ApiResponse<bool>
                {
                    success = false,
                    status = 500,
                    message = "Internal Server Error",
                    data = false
                };
            }
        }
    }
}
