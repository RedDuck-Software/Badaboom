using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Models;
using BackendCore.Services;
using System.Security.Claims;

namespace Badaboom.Backend.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JWTAuth _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<JWTAuth> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await attachUserToContext(context, userService, token);

            await _next(context);
        }

        private async Task attachUserToContext(HttpContext context, IUserService userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidIssuer = _appSettings.Issuer,
                    ValidateIssuer = true, 
                    ValidAudience = _appSettings.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = _appSettings.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userAddress = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;

                // attach user to context on successful jwt validation
                context.Items["User"] = await userService.GetUserByAddress(userAddress);
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}