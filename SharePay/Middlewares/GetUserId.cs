using SharePay.Models;
using SharePay.Security;

namespace SharePay.Middlewares
{
    public class GetUserId : IMiddleware
    {
        private readonly ILogger<GetUserId> logger;
        private UserCreds creds;
        public GetUserId(ILogger<GetUserId> logger, UserCreds creds)
        {
            this.logger = logger;
            this.creds = creds;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.StartsWithSegments("/api/User") && context.Request.Method.ToUpperInvariant() == "POST")
            {
                await next(context);
            }

            string bearerToken = context.Request.Headers["Authorization"];
            if(!string.IsNullOrWhiteSpace(bearerToken) || !bearerToken.StartsWith("Bearer"))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Header Format");
                return;
            }

            string token = bearerToken.Split(' ')[1];
            int userId = int.Parse(TokenDecoder.DecodeToken(token));
            creds.UserId = userId;

            await next(context);
        }
    }
}
