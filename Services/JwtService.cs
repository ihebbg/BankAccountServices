using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BankAccountServices.DTOs.User;
using BankAccountServices.Entities;
using BankAccountServices.Repositories.Interfaces;
using BankAccountServices.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BankAccountServices.Services
{
	public class JwtService(IConfiguration config, IJwtRepository jwtRepository) : IJwtService
	{
		private readonly IConfiguration _config=config;
		private readonly IJwtRepository _jwtRepository=jwtRepository;

		public string CreateToken(UserLogin userToken)
		{
			var jwtSettings = _config.GetSection("Jwt");


			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
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

		public string CreateSaveRefreshToken(long idUser)
		{
			if (idUser <= 0)
			{
				throw new ArgumentException("Le type doit être un nombre positif.", nameof(idUser));
			}

			_jwtRepository.RevokeRefreshTokenByUser(idUser);
			string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


			_jwtRepository.SaveRefreshToken(idUser, refreshToken);
			return refreshToken;

		}

		public bool ValidateRefreshToken(string refreshToken)
		{
			if(string.IsNullOrEmpty(refreshToken))
			{
				throw new ArgumentException("refreshToken ne doit pas etre null ou vide");

			}
			var refresh=_jwtRepository.GetRefreshToken(refreshToken);
			if(refresh == null)
			{
				throw new KeyNotFoundException($"Aucune refresh token trouvé");
			}
			return !refresh.IsRevoked && refresh.DateExpiration > DateTime.Now;
		}
	}
}
