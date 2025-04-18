using Dapper;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using SharePay.Interfaces;
using SharePay.Models;
using SharePay.Models.ViewModels;
using SharePay.Security;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SharePay.Services
{
    public class UserServices : IUserServices
    {
        private readonly IDbConnection connection;
        private readonly ILogger<UserServices> logger;
        private readonly IConfiguration config;
        private readonly UserCreds creds;
        public UserServices(IConfiguration config, ILogger<UserServices> logger, UserCreds creds)
        {
            connection = new MySqlConnection(config.GetConnectionString("DefaultConnection"));
            this.logger = logger;
            this.config = config;
            this.creds = creds;
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

                string hashedPassword = PasswordHasher.HashPassword(user.password);

                var result = await connection.ExecuteAsync("INSERT INTO Users (email, username, password) VALUES (@email, @username, @password)", new { email = user.email, username = user.username, password=hashedPassword });
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


        public async Task<ApiResponse<object>> Login(Users user)
        {
            try
            {
                Users existingUser = await connection.QueryFirstOrDefaultAsync<Users>("SELECT * FROM users WHERE email=@email", new { email = user.email });
                if (existingUser == null)
                {
                    return new ApiResponse<object>
                    {
                        success = false,
                        message = "User does not exist, Please Register",
                        data = new { token = string.Empty },
                        status = 400
                    };
                }
                bool isVerified = PasswordHasher.verifyPassword(user.password, existingUser.password);
                if (isVerified)
                {
                    return new ApiResponse<object>
                    {
                        success = true,
                        message = "Logged in",
                        status = 200,
                        data = new { token = getToken(existingUser) }
                    };
                }
                return new ApiResponse<object>
                {
                    success = false,
                    message = "Wrong Password",
                    status = 400,
                    data = new { token = string.Empty }
                };

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new ApiResponse<object>
                {
                    success = false,
                    message = "fatal error occured",
                    status = 500,
                    data = new { token = string.Empty }
                };
            }
        }

        public async Task<ApiResponse<List<UsersVM>>> GetAllUsers()
        {
            try
            {
                var users = await connection.QueryAsync<UsersVM>("SELECT id, username, email FROM users WHERE id != @id", new { id = creds.UserId });
                return new ApiResponse<List<UsersVM>>
                {
                    success = true,
                    status = 200,
                    message = "List of All Users",
                    data = users.ToList()
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new ApiResponse<List<UsersVM>>
                {
                    success = false,
                    message = "Internal Server Error",
                    data = new List<UsersVM>(),
                    status = 500
                };
            }
        }

        public async Task<ApiResponse<UsersVM>> SearchUser(string searchPromt)
        {
            try
            {
                var user = await connection.QueryFirstAsync<UsersVM>("SELECT id, username, email FROM users WHERE username LIKE @search OR email LIKE @search", new { search = "%" + searchPromt + "%" });
                if(user == null)
                {
                    return new ApiResponse<UsersVM>
                    {
                        success = true,
                        status = 401,
                        message = "User not found!",
                        data = new UsersVM()
                    };
                }
                return new ApiResponse<UsersVM>
                {
                    status = 200,
                    data = user,
                    message = "User Found",
                    success = true
                };
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return new ApiResponse<UsersVM>
                {
                    success = false,
                    message = "Internal Server Error",
                    status = 500,
                    data = new UsersVM()
                };
            }
        }

        private string getToken(Users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["jwt:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString())
            };
            var token = new JwtSecurityToken(
               signingCredentials: credentials,
               claims: claims,
               expires: DateTime.UtcNow.AddDays(1));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
