using BankAccountServices.Data;
using BankAccountServices.Entities;
using BankAccountServices.Repositories.Interfaces;

namespace BankAccountServices.Repositories
{
	public class JwtRepository(AppDbContext appDbContext) : IJwtRepository
	{
		private readonly AppDbContext _appDbContext=appDbContext;

		public void RevokeRefreshTokenByUser(long idUser)
		{
			var refreshToken = _appDbContext.RefreshTokens.Where(x => x.IdUser == idUser && !x.IsRevoked && x.DateExpiration > DateTime.Now).ToList();
			if (refreshToken.Any())
			{
				foreach (var r in refreshToken)
				{
					r.IsRevoked = true;
					_appDbContext.SaveChanges();
				}
			}
		}

		public void SaveRefreshToken(long idUser,string refreshToken)
		{
			RefreshToken input = new()
			{
				
				DateExpiration = DateTime.Now.AddDays(30),
				IsRevoked = false,
				Token = refreshToken,
				IdUser = idUser,
				DateCreation = DateTime.Now,
			

			};
			_appDbContext.RefreshTokens.Add(input);
			_appDbContext.SaveChanges();
		}

		public RefreshToken GetRefreshToken(string refreshToken)
		{
			return _appDbContext.RefreshTokens.Where(x => x.Token == refreshToken).FirstOrDefault()!;
		
		}
	}
}
