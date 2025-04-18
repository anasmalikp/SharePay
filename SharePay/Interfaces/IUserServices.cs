using SharePay.Models;
using SharePay.Models.ViewModels;

namespace SharePay.Interfaces
{
    public interface IUserServices
    {
        Task<ApiResponse<bool>> RegisterUser(Users user);
        Task<ApiResponse<object>> Login(Users user);
        Task<ApiResponse<List<UsersVM>>> GetAllUsers();
        Task<ApiResponse<UsersVM>> SearchUser(string searchPromt);
    }
}
