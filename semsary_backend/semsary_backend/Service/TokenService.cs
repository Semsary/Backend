using Microsoft.IdentityModel.Tokens;
using semsary_backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace semsary_backend.Service
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config,IHttpContextAccessor httpContextAccessor) 
        {
            _config = config;
            this.httpContextAccessor = httpContextAccessor;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:Key"]));
        }
        public string GenerateToken(semsary_backend.Models.SermsaryUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim("username",user.Username),
                new Claim("role",user.UserType.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Ulid.NewUlid().ToString())
            };
            var cred=new SigningCredentials(_key,SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Token:issuer"],
                audience: _config["Token:issuer"],
                claims = claims,
                expires: DateTime.UtcNow.AddYears(1),
                signingCredentials:cred
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
            
        }
        public string? GetCurUser()
        {
            var username = httpContextAccessor.HttpContext?.User.FindFirst("username")?.Value;
            return username;


        }
    }
}
