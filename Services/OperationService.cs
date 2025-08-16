using AutoMapper;
using BankAccountServices.DTOs;
using BankAccountServices.DTOs.Operation;
using BankAccountServices.Entities;
using BankAccountServices.Repositories.Interfaces;
using BankAccountServices.Services.Interfaces;

namespace BankAccountServices.Services
{
	public class OperationService(IOperationRepository repository, IMapper mapper, IBankAccountService bankAccountService) : IOperationService
	{
		private readonly IOperationRepository _repository = repository;
		private readonly IBankAccountService _bankAccountService = bankAccountService;
		private readonly IMapper _mapper = mapper;
		public Retour AddOperation(OperationCreatedDTO operation)
		{
			Retour r = new();
			var input = _mapper.Map<AccountOperation>(operation);
			long idOperation = _repository.AddOperation(input);
			r.ID = idOperation;
			r.Code = CodeRetour.Ok;
			r.Message = "Operation added";
			return r;


		}

		public Retour CrediBankAccount(long idBankAccount, double amount)
		{
			Retour r = new();
			if (amount < 0)
			{
				throw new ArgumentException("Amount doit etre positif", nameof(amount));
			}
			var bankAccountGet = _bankAccountService.GetBankAccountById(idBankAccount);
			if (bankAccountGet != null)
			{
				var bankAccount = _mapper.Map<BankAccount>(bankAccountGet);
				_repository.CreditAccount(bankAccount, amount);
				OperationCreatedDTO operation = new OperationCreatedDTO()
				{
					Amount = amount,
					IdBankAccount = idBankAccount,
					Type = Enums.OperationsType.CREDIT,

				};
				 r= AddOperation(operation);

			}
			return r;

		}

		public Retour DebitBankAccount(long idBankAccount, double amount)
		{
			Retour r = new();
			if (amount < 0)
			{
				throw new ArgumentException("Amount doit etre positif", nameof(amount));
			}
			var bankAccountGetd = _bankAccountService.GetBankAccountById(idBankAccount);
			if (bankAccountGetd != null)
			{
				var bankAccount = _mapper.Map<BankAccount>(bankAccountGetd);
				if (_repository.VerifSoldeDebit(bankAccount, amount))
				{
					_repository.DebitAccount(bankAccount, amount);
					OperationCreatedDTO operation = new OperationCreatedDTO()
					{
						Amount = amount,
						IdBankAccount = idBankAccount,
						Type = Enums.OperationsType.CREDIT,

					};
					return AddOperation(operation);
				}
				else
				{
					r.Code = CodeRetour.Ko;
					r.Message = "Solde insuffisant";
				}
			}
			return r;

		}

		public List<OperationResponseDTO> GetAllOperatiosn()
		{
			var operations = _repository.GetAllOperations();
			return _mapper.Map<List<OperationResponseDTO>>(operations);
		}

		public OperationResponseDTO GetOerationById(long idIdOperation)
		{
			if (idIdOperation <= 0)
			{
				throw new ArgumentException("Le type doit être un nombre positif.", nameof(idIdOperation));

			}
			var operation = _repository.GetOperatioById(idIdOperation);
			if (operation == null)
				throw new KeyNotFoundException($"Aucune opération trouvée avec l'ID {idIdOperation}.");
			return _mapper.Map<OperationResponseDTO>(operation);

		}
	}

}
