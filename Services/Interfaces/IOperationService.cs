using BankAccountServices.DTOs;
using BankAccountServices.DTOs.Operation;

namespace BankAccountServices.Services.Interfaces
{
	public interface IOperationService
	{
		Retour AddOperation(OperationCreatedDTO operation);
		List<OperationResponseDTO> GetAllOperatiosn();
		OperationResponseDTO GetOerationById(long idIdOperation);
		Retour CrediBankAccount(long bankAccountId, double amount);
		Retour DebitBankAccount(long bankAccountId, double amount);


	}
}
