using BankAccountServices.Entities;

namespace BankAccountServices.Repositories.Interfaces
{
	public interface IOperationRepository
	{
		IEnumerable<AccountOperation> GetAllOperations();
		AccountOperation? GetOperatioById(long idOperation);
		void CreditAccount(BankAccount bankAccount, double amount);
		void DebitAccount(BankAccount bankAccount, double amount);
		long AddOperation(AccountOperation operation);
		bool VerifSoldeDebit(BankAccount bankAccount, double amount);
	}
}
