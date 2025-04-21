using System.Data;
using AutoMapper;
using Dapper;
using MySql.Data.MySqlClient;
using SharePay.Interfaces;
using SharePay.Models;
using SharePay.Models.ViewModels;

namespace SharePay.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ILogger<ExpenseService> logger;
        private readonly IDbConnection connection;
        private readonly UserCreds creds;
        private readonly IMapper mapper;

        public ExpenseService(ILogger<ExpenseService> logger, IConfiguration config, UserCreds creds, IMapper mapper)
        {
            this.logger = logger;
            this.creds = creds;
            connection = new MySqlConnection(config.GetConnectionString("DefaultConnection"));
            this.mapper = mapper;
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

                //int expId = await connection.ExecuteScalarAsync<int>("INSERT INTO expenses (amount, isSettled, Note, name, paidBy) VALUES (@amount, @isSettled, @Note, @name, @paidBy)", new { amount = expense.amount, isSettled = false, Note = expense.Note, name = expense.name, paidBy=creds.UserId });
                int expId = await connection.ExecuteScalarAsync<int>(
                    @"INSERT INTO expenses (amount, isSettled, Note, name, paidBy) VALUES (@amount, @isSettled, @Note, @name, @paidBy);
                    SELECT LAST_INSERT_ID();", new { amount = expense.amount, isSettled = false, Note = expense.Note, name = expense.name, paidBy = creds.UserId }
                    );
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
                    await connection.ExecuteAsync("INSERT INTO userexpenses (expId, paidAmt, userAmount, userId) VALUES (@expId, @paidAmt, @userAmount, @userId)", new { expId = expId, paidAmt = usr == creds.UserId ? expense.amount/expense.users.Count : 0, userAmount = expense.amount/expense.users.Count, userId = usr});
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

        public async Task<ApiResponse<IEnumerable<Expenses>>> GetUserPaidExpenses()
        {
            try
            {
                var userId = creds.UserId;
                var expenses = await connection.QueryAsync<Expenses>("SELECT * FROM expenses WHERE paidBy = @paidBy", new { paidby = userId });

                if (!expenses.Any())
                {
                    return new ApiResponse<IEnumerable<Expenses>>
                    {
                        success = false,
                        message = "No Expenses Found!",
                        status = 401,
                        data = new List<Expenses>()
                    };
                }
                return new ApiResponse<IEnumerable<Expenses>>
                {
                    data = expenses,
                    message = "list of expenses",
                    status = 200,
                    success = true
                };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new ApiResponse<IEnumerable<Expenses>>
                {
                    success = false,
                    status = 500,
                    message = "Internal Server Error",
                    data = new List<Expenses>()
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<UserExpenseVM>>> GetExpenseDetails(int expId)
        {
            try
            {
                List<UserExpenseVM> exp = new List<UserExpenseVM>();
                var expenses = await connection.QueryAsync("SELECT u.id as id, u.paidAmt as paidAmt, u.userAmount as userAmount, s.id as userId, s.email as email, s.username as username FROM userexpenses as u INNER JOIN users as s on u.userId = s.id WHERE u.expId = @expId", new { expId = expId });
                if (!expenses.Any())
                {
                    return new ApiResponse<IEnumerable<UserExpenseVM>>
                    {
                        message = "No Data Found!",
                        data = new List<UserExpenseVM>(),
                        status = 400,
                        success = false
                    };
                }

                foreach (var e in expenses)
                {
                    UserExpenseVM data = new UserExpenseVM
                    {
                        paidAmt = e.paidAmt,
                        userAmount = e.userAmount,
                        id = e.id,
                        userDetails = new Users
                        {
                            email = e.email,
                            username = e.username,
                            id = e.userId
                        }
                    };

                    exp.Add(data);
                }

                return new ApiResponse<IEnumerable<UserExpenseVM>>
                {
                    data = exp,
                    message = "Expense Details",
                    status = 200,
                    success = true
                };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.ToString());
                return new ApiResponse<IEnumerable<UserExpenseVM>>
                {
                    message = "Internal Server Error",
                    data = new List<UserExpenseVM>(),
                    success = false,
                    status = 500
                };
            }
        }
    }
}
