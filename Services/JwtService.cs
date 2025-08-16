using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankAccountServices.DTOs.User;
using BankAccountServices.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BankAccountServices.Services
{
	public class JwtService : IJwtService
	{
		private readonly IConfiguration _config;

		public JwtService(IConfiguration config)
		{
			_config = config;
		}

		public string CreateToken(UserToken userToken)
		{
			var jwtSettings = _config.GetSection("Jwt");


			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			// Création des claims
			var claims = new List<Claim>
				{
					new Claim("jwtLogin", userToken.Login),
					new Claim(ClaimTypes.Role, userToken.Role),
				};

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = jwtSettings["Issuer"],
				Audience = jwtSettings["Audience"],
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = creds,
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
