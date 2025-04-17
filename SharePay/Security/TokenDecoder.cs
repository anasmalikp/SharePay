using System.IdentityModel.Tokens.Jwt;

namespace SharePay.Security
{
    public class TokenDecoder
    {
        public static string DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var decoded = handler.ReadJwtToken(token);
            var userId = decoded.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            return userId;
        }
    }
}
