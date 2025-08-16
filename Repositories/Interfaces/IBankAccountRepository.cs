using BankAccountServices.Entities;
using BankAccountServices.Enums;

namespace BankAccountServices.Repositories.Interfaces
{
	public interface IBankAccountRepository
	{
		long AddBankAccountSaving(SavingAccount bankAccountSaving);
		long AddBankAccountCurrent(CurrentAccount bankAccountCurrent);
		IEnumerable<BankAccount> GetAllBankAccount();
		IQueryable<BankAccount> GetAllBankAccountByStatut(AccountStatus status);
		IQueryable<BankAccount> GetAllBankAccountByType(string type);

		BankAccount GetBankAccount(long idBankAccount);

		void UpdateBankAccount(BankAccount newBankAccount, long idBankAccount);
		void DeleteBankAccount(BankAccount bankAccount);
	}
}
