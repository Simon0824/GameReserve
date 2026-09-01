using System.Security.Claims;
using System.Text;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;


namespace Identity.Infrastructure.Auth;

public class TokenProvider : ITokenProvider
{
    private readonly SigningCredentials _credentials;
    private readonly JsonWebTokenHandler _handler;
    private readonly string _audience;
    private readonly string _issuer;
    public TokenProvider(IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:SecretKey"] 
                                ?? throw new InvalidOperationException("Secret key is missing in configuratior");

        _audience = configuration["Jwt:Audience"] 
                                ?? throw new InvalidOperationException("Audience is missing in configuratior");

        _issuer = configuration["Jwt:Issuer"] 
                                ?? throw new InvalidOperationException("Issuer is missing in configuratior");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        _credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        _handler = new JsonWebTokenHandler();
    }
    public string CreateToken(User user)
    {

        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!)
            ]),
            SigningCredentials = _credentials,
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = _issuer,
            Audience = _audience
        };
        var token = _handler.CreateToken(descriptor);

        return token;
    }
}
