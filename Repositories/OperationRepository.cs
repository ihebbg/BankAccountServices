using BankAccountServices.Data;
using BankAccountServices.Entities;
using BankAccountServices.Repositories.Interfaces;

namespace BankAccountServices.Repositories
{
	public class OperationRepository(AppDbContext appDbContext) : IOperationRepository

	{
		private readonly AppDbContext _appDbContext = appDbContext;

		public long AddOperation(AccountOperation operation)
		{
			operation.Date = DateTime.Now;
			_appDbContext.AccountOperations.Add(operation);
			_appDbContext.SaveChanges();
			return operation.IdAccountOperation;

		}

		public void CreditAccount(BankAccount bankAccount, double amount)
		{
			bankAccount.Balance = bankAccount.Balance + amount;

			_appDbContext.SaveChanges();
		}

		public void DebitAccount(BankAccount bankAccount, double amount)
		{
			bankAccount.Balance = bankAccount.Balance - amount;
			_appDbContext.SaveChanges();
		}

		public IEnumerable<AccountOperation> GetAllOperations()
		{
			return _appDbContext.AccountOperations.ToList();
		}

		public AccountOperation? GetOperatioById(long idOperation)
		{
			return _appDbContext.AccountOperations.Find(idOperation);
		}

		public bool VerifSoldeDebit(long idBankAccount, double amount)
		{
			return _appDbContext.BankAccounts.Find(idBankAccount)?.Balance >= amount;

		}

		public bool VerifSoldeDebit(BankAccount bankAccount, double amount)
		{
			return bankAccount.Balance >= amount;
		}
	}
}
