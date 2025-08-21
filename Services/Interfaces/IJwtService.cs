using BankAccountServices.DTOs.User;
using BankAccountServices.Entities;

namespace BankAccountServices.Services.Interfaces
{
	public interface IJwtService
	{
		string CreateToken(UserLogin userToken);
		string CreateSaveRefreshToken(long idUser);
		bool ValidateRefreshToken(string refreshToken);
		 
	}
}
