using BankAccountServices.DTOs.User;
using BankAccountServices.Entities;

namespace BankAccountServices.Repositories.Interfaces
{
	public interface IJwtRepository
	{
		void SaveRefreshToken(long idUser, string refreshToken);
		void RevokeRefreshTokenByUser(long idUser);
		RefreshToken GetRefreshToken(string  refreshToken);
	}
}
