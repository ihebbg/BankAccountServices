using BankAccountServices.DTOs.User;
using BankAccountServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankAccountServices.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class Authentification(IJwtService jwtService) : ControllerBase
	{
		private IJwtService _jwtService = jwtService;

		[HttpPost("login")]
		public IActionResult Login(UserLogin user)
		{
			var token = _jwtService.CreateToken(user);
			var refreshToken = _jwtService.CreateSaveRefreshToken(user.IdUser);
			return Ok(new
			{
				token,
				refreshToken
			});
		}

		[HttpPost("refresh")]
		public IActionResult Refresh(string refreshToken, UserLogin user)
		{
			var isValid = _jwtService.ValidateRefreshToken(refreshToken);
			if (!isValid) return Unauthorized("Invalid or expired refresh token");


			var newAccessToken = _jwtService.CreateToken(user);
			var newRefreshToken = _jwtService.CreateSaveRefreshToken(user.IdUser);

			return Ok(new
			{
				accessToken = newAccessToken,
				refreshToken = newRefreshToken
			});
		}
	}
}
