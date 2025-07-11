using Microsoft.IdentityModel.Tokens;
using ExpenseTracker.Application.Contracts.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseTracker.Domain.Utils;
using ExpenseTracker.Application.DTO.User;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Web.Auth;

public class TokenManager : ITokenManager
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TokenManager> _logger;
    public TokenManager(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<TokenManager> logger)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public bool GenerateTokensAndSetCookies(LoggedInUserDTO user, bool isPersistent)
    {
        var token = GenerateAccessToken(user, isPersistent);
        if (!string.IsNullOrEmpty(token))
        {
            return SetAuthenticationCookies(token, isPersistent);
        }
        return false;
    }

    private string GenerateAccessToken(LoggedInUserDTO user, bool isPersistent)
    {
        try
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var authClaims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: isPersistent? DateTime.Now.AddDays(30) : DateTime.Now.AddHours(24),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while generating the access token for the user - {0}.", user.Email);
        }
        return string.Empty;
    }

    private bool SetAuthenticationCookies(string accessToken, bool isPersistent)
    {
        try
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Set the JWT in an HTTP-only cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // Makes it inaccessible to JavaScript
                    Secure = true,   // Ensures the cookie is only sent over HTTPS
                    IsEssential = true,
                    SameSite = SameSiteMode.Strict, // Protects from CSRF
                    Expires = isPersistent ? DateTime.Now.AddDays(30) : DateTime.Now.AddHours(24), // Set expiration to match token lifetime
                };

                _httpContextAccessor.HttpContext?.Response.Cookies.Append(Constants.ACCESS_TOKEN_NAME, accessToken, cookieOptions);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while adding the Authentication cookies.");
        }
        return false;

    }
}
