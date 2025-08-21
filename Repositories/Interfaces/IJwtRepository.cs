using BankAccountServices.DTOs.User;

namespace BankAccountServices.Repositories.Interfaces
{
	public interface IJwtRepository
	{
		void SaveRefreshToken(long idUser, string refreshToken);
		void RevokeRefreshTokenByUser(long idUser);
	}
}
