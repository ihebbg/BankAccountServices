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
		[HttpPost("create/token")]
		public IActionResult CreateToken(UserToken user)
		{
			return Ok(_jwtService.CreateToken(user));
		}
	}
}
