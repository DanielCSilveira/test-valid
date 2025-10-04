using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.IntegrationTests.Helpers
{
    public static class JwtTokenHelper
    {
        private static string GenerateTestToken(
            string username = "testuser",
            string role = "user",
            string issuer = "http://localhost",
            string audience = "my-api",
            string secretKey = "super-secret-test-key-12345123123123123123123123"
        )
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("preferred_username", username),
            new Claim(ClaimTypes.Role, role)
        };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async static Task<string> GenerateValidToken(IConfiguration configuration
      )
        {

            var issuer = configuration.GetValue<string>("Authentication:Authority")
                         ?? configuration.GetValue<string>("Keycloak:Authority")
                         ?? "";

            var audience = configuration.GetValue<string>("Authentication:Audience")
                           ?? configuration.GetValue<string>("Keycloak:ClientId")
                           ?? "";

            var secretKey = configuration.GetValue<string>("Keycloak:client_secret")
                            ?? "";
            var grant_type = configuration.GetValue<string>("Keycloak:grant_type")
                            ?? "";

            var username = configuration.GetValue<string>("Keycloak:test-user:username") 
                            ?? "";
            var password = configuration.GetValue<string>("Keycloak:test-user:password") 
                            ?? "";
            
            var urlAuthentication = issuer + @"/protocol/openid-connect/token";
            var form = new Dictionary<string, string>
            {
                ["client_id"] = audience,
                ["client_secret"] = secretKey,
                ["username"] = username,
                ["password"] = password,
                ["grant_type"] = grant_type
            };
            using var client = new HttpClient();
            
            var response = await client.PostAsync(
                          urlAuthentication,
                          new FormUrlEncodedContent(form));

            response.EnsureSuccessStatusCode();
            var tokenResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            return tokenResponse.GetProperty("access_token").GetString() ?? "";


        }

        public static string GenerateInvalidToken()
        {
            return JwtTokenHelper.GenerateTestToken();
        }
    }
}
