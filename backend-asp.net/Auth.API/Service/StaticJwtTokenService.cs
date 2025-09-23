using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.API.Model;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.JwtSetting;

namespace Auth.API.Service;

public static class StaticJwtTokenService
{
    public static string GenerateJwtToken(UserModel user)
    {
        var key = JwtSetting.Key;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),

            Expires = DateTime.UtcNow.AddDays(7),

            // sign the token with secret
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = JwtSetting.Issuer,
            Audience = JwtSetting.Audience,

        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}