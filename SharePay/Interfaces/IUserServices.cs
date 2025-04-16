using SharePay.Models;

namespace SharePay.Interfaces
{
    public interface IUserServices
    {
        Task<ApiResponse<bool>> RegisterUser(Users user);
        Task<ApiResponse<object>> Login(Users user);
    }
}
