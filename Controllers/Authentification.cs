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
			var token=_jwtService.CreateToken(user);
			var refreshToken=_jwtService.CreateSaveRefreshToken(user.IdUser);
			return Ok(new
			{
				token,
				refreshToken
			});
		}
	}
}
