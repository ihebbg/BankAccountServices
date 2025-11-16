using AutoMapper;
using BankAccountServices.Data;
using BankAccountServices.Entities;
using BankAccountServices.Enums;
using BankAccountServices.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankAccountServices.Repositories
{
	public class BankAccountRepository(AppDbContext appDbContext) : IBankAccountRepository
	{

		private readonly AppDbContext _appDbContext = appDbContext;

		public long AddBankAccountCurrent(CurrentAccount bankAccountCurrnet)

		{
			bankAccountCurrnet.Created = DateTime.Now;
			_appDbContext.BankAccounts.Add(bankAccountCurrnet);
			_appDbContext.SaveChanges();
			return bankAccountCurrnet.IdBankAccount;
		}



		public long AddBankAccountSaving(SavingAccount bankAccountSaving)
		{
			bankAccountSaving.Created = DateTime.Now;
			_appDbContext.BankAccounts.Add(bankAccountSaving);
			_appDbContext.SaveChanges();
			return bankAccountSaving.IdBankAccount;
		}



		public IEnumerable<BankAccount> GetAllBankAccount()
		{
			return _appDbContext.BankAccounts.Include(c=>c.Customer)
								  .ToList();
		}

		public IQueryable<BankAccount> GetAllBankAccountByStatut(AccountStatus status)
		{
			return _appDbContext.BankAccounts.Where(x => x.Status == status);
		}

		public IQueryable<BankAccount> GetAllBankAccountByType(string type)
		{
			return type.ToLower() switch
			{
				"saving" => _appDbContext.Set<SavingAccount>().AsQueryable(),
				"current" => _appDbContext.Set<CurrentAccount>().AsQueryable(),
				_ => Enumerable.Empty<BankAccount>().AsQueryable(),
			};
		}

		public BankAccount GetBankAccount(long idBankAccount)
		{
			return _appDbContext.BankAccounts.Find(idBankAccount)!;
		}



		public void DeleteBankAccount(BankAccount bankAccount)
		{
			_appDbContext.Remove(bankAccount);
			_appDbContext.SaveChanges();
		}

		public void UpdateBankAccount(BankAccount newBankAccount, long idBankAccount)
		{
			throw new NotImplementedException();
		}
	
	}
}
