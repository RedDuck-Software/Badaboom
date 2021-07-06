using Backend.Models;
using BackendCore.Models;
using BackendCore.Models.Request;
using BackendCore.Models.Response;
using Database.Models;
using Database.Respositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BackendCore.Services
{
    public interface IUserService
    {
        Task<RegisterResponse> Register(RegisterRequest model);
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
        Task<bool> RevokeToken(string token);
        Task<User> GetUserByAddress(string address);
    }

    public class UserService : IUserService
    {
        private readonly JWTAuth _appSettings;
        private readonly IConfiguration _configuration;
        private readonly INonceGeneratorService _nonceGeneratorService;
        private readonly string _connectionString;

        public UserService(
            IOptions<JWTAuth> appSettings,
            IConfiguration configuration,
            INonceGeneratorService nonceGeneratorService // todo: register service in ConfigureServices
            )
        {
            _appSettings = appSettings.Value;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _nonceGeneratorService = nonceGeneratorService;
        }

        public async Task<RegisterResponse> Register(RegisterRequest model)
        {
            User user;

            using (var uRepo = new UserRepository(_connectionString))
                user = await uRepo.GetUserByAddress(model.Address);

            // return null if user not found
            if (user != null) return null;

            user = new User { Address = model.Address, Nonce = _nonceGeneratorService.GenerateNonce() };

            using (var uRepo = new UserRepository(_connectionString))
                await uRepo.CreateUser(user);

            return new RegisterResponse() { Nonce = user.Nonce };
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            User user;

            using (var uRepo = new UserRepository(_connectionString))
                user = await uRepo.GetUserByAddress(model.Address);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);

            refreshToken.UserId = user.UserId;

            // save refresh token
            using (var uRepo = new UserRepository(_connectionString))
                await uRepo.AddNewRefreshToken(refreshToken);

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            RefreshToken refreshToken;

            using (var uRepo = new UserRepository(_connectionString))
                refreshToken = await uRepo.GetRefreshTokenWithUser(token);

            // return null if no user found with token
            if (refreshToken == null) return null;

            if(!refreshToken.IsActive)
            {
                await RemoveToken(refreshToken.Token);

                return null;
            }

            // generate new jwt
            var jwtToken = GenerateJwtToken(refreshToken.User);

            return new AuthenticateResponse(refreshToken.User, jwtToken, refreshToken.Token);
        }

        public async Task<bool> RevokeToken(string token)
        {
            RefreshToken refreshToken;

            using (var uRepo = new UserRepository(_connectionString))
                refreshToken = await uRepo.GetRefreshTokenWithUser(token);

            // return false if no user found with token
            if (refreshToken == null) return false;

            await RemoveToken(refreshToken.Token);

            return true;
        }

        public async Task<User> GetUserByAddress(string contractAddress)
        {
            using var uRepo = new UserRepository(_connectionString);
            return await uRepo.GetUserByAddress(contractAddress);
        }


        private async Task RemoveToken(string refreshToken)
        {
            using var uRepo = new UserRepository(_connectionString);
            await uRepo.RemoveRefreshToken(refreshToken);
        }


        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Address)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.AccessTokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(_appSettings.RefreshTokenLifetime),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}
