using BankAccountServices.DTOs.User;

namespace BankAccountServices.Services.Interfaces
{
	public interface IJwtService
	{
		string CreateToken(UserLogin userToken);
		string CreateSaveRefreshToken(long idUser);
		 
	}
}
