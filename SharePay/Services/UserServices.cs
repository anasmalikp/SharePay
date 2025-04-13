using Dapper;
using MySql.Data.MySqlClient;
using SharePay.Interfaces;
using SharePay.Models;
using System.Data;

namespace SharePay.Services
{
    public class UserServices : IUserServices
    {
        private readonly IDbConnection connection;
        private readonly ILogger<UserServices> logger;
        public UserServices(IConfiguration config, ILogger<UserServices> logger)
        {
            connection = new MySqlConnection(config.GetConnectionString("DefaultConnection"));
            this.logger = logger;
        }

        public async Task<ApiResponse<bool>> RegisterUser(Users user)
        {
            try
            {
                Users existingUser = await connection.QueryFirstOrDefaultAsync<Users>("SELECT id FROM Users WHERE email=@email", new { email = user.email });
                if (existingUser != null)
                {
                    return new ApiResponse<bool>
                    {
                        success = false,
                        message = "User already exists",
                        data = false,
                    };
                }

                var result = await connection.ExecuteAsync("INSERT INTO Users (email, username) VALUES (@email, @username)", new { email = user.email, username = user.username });
                if (result > 0)
                {
                    return new ApiResponse<bool>
                    {
                        success = true,
                        message = "User registered successfully",
                        data = true,
                    };
                }
                else
                {
                    return new ApiResponse<bool>
                    {
                        success = false,
                        message = "User registration failed",
                        data = false,
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering user");
                return new ApiResponse<bool>
                {
                    success = false,
                    message = "An error occurred while registering the user",
                    data = false,
                };
            }
        }
    }
}
