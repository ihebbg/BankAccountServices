using AutoMapper;
using BankAccountServices.DTOs;
using BankAccountServices.DTOs.BankAccount;
using BankAccountServices.DTOs.BankAccount.CurrentBankAccount;
using BankAccountServices.DTOs.BankAccount.SavingBankAccount;
using BankAccountServices.Entities;
using BankAccountServices.Enums;
using BankAccountServices.Repositories.Interfaces;
using BankAccountServices.Services.Interfaces;

namespace BankAccountServices.Services
{
	public class BankAccountService(IBankAccountRepository repository, IMapper mapper) : IBankAccountService
	{
		private readonly IBankAccountRepository _repository = repository;
		private readonly IMapper _mapper = mapper;

		public Retour AddBankAccountCurrent(CurrentBankAccountCreateDTO bankAccount)
		{
			Retour r = new();

			var input = _mapper.Map<CurrentAccount>(bankAccount);
			var id = _repository.AddBankAccountCurrent(input);
			r.Code = CodeRetour.Ok;
			r.Message = "Bank Account Added";
			r.ID = id;
			return r;


		}

		public Retour AddBankAccountSaving(SavingBankAccountCreateDTO bankAccount)
		{
			Retour r = new();

			var input = _mapper.Map<SavingAccount>(bankAccount);
			var id = _repository.AddBankAccountSaving(input);
			r.Code = CodeRetour.Ok;
			r.Message = "Bank Account Added";
			r.ID = id;
			return r;


		}

		public Retour DeleteBankAccount(long idBankAccount)
		{
			Retour r = new();

			var bankAccount = GetBankAccountById(idBankAccount);
			if (bankAccount != null)
			{
				var bankAccountEntity = _mapper.Map<BankAccount>(bankAccount);
				_repository.DeleteBankAccount(bankAccountEntity);
				r.Code = CodeRetour.Ok;
				r.Message = "Account deleted";
			
			}
			return r;


		}

		public List<BankAccountResponseDTO> GetAllBankAccounts()
		{

			var accounts = _repository.GetAllBankAccount();
			return _mapper.Map<List<BankAccountResponseDTO>>(accounts);



		}

		public BankAccountResponseDTO GetBankAccountById(long idBankAccount)
		{
			if (idBankAccount <= 0)
				throw new ArgumentException("L'identifiant doit être un nombre positif.", nameof(idBankAccount));
			var bankAccount = _repository.GetBankAccount(idBankAccount);


			if (bankAccount == null)
				throw new KeyNotFoundException($"Aucun bank account trouvée avec l'ID {idBankAccount}.");
			return _mapper.Map<BankAccountResponseDTO>(bankAccount);

		}

		public List<BankAccountResponseDTO> GetBankAccountByStatus(AccountStatus status)
		{


			var bankAccount = _repository.GetAllBankAccountByStatut(status);
			return _mapper.Map<List<BankAccountResponseDTO>>(bankAccount);



		}

		public List<BankAccountResponseDTO> GetBankAccountByType(string type)
		{

			var bankAccount = _repository.GetAllBankAccountByType(type);
			return _mapper.Map<List<BankAccountResponseDTO>>(bankAccount);

		}

		public List<BankAccountResponseDTO> GetPaginatedBankAccount(int page, int pageSize)
		{

			var bankAccounts = _repository.GetAllBankAccount().Skip((page - 1) * pageSize)
		.Take(pageSize);

			return _mapper.Map<List<BankAccountResponseDTO>>(bankAccounts);



		}

	}
}
