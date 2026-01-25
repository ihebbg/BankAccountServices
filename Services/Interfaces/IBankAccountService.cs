using BankAccountServices.DTOs.Customer;
using BankAccountServices.DTOs;
using BankAccountServices.DTOs.BankAccount;
using BankAccountServices.DTOs.BankAccount.SavingBankAccount;
using BankAccountServices.DTOs.BankAccount.CurrentBankAccount;
using BankAccountServices.Enums;

namespace BankAccountServices.Services.Interfaces
{
	public interface IBankAccountService
	{
		Retour AddBankAccountSaving(SavingBankAccountCreateDTO bankAccount); 
		Retour AddBankAccountCurrent(CurrentBankAccountCreateDTO bankAccount);
		BankAccountResponseDTO GetBankAccountById(long idBankAccount);
		Retour DeleteBankAccount(long idBankAccount);
		List<BankAccountResponseDTO> GetAllBankAccounts();
		List<BankAccountResponseDTO> GetBankAccountByStatus(AccountStatus status);
		List<BankAccountResponseDTO> GetBankAccountByType(string type);
		List<BankAccountResponseDTO> GetPaginatedBankAccount(int page, int pageSize);
		Retour UpdateAccount(int idBankAccount);

	}
}
