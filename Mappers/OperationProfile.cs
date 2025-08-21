using AutoMapper;
using BankAccountServices.DTOs.BankAccount;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.DTOs.Operation;
using BankAccountServices.Entities;

namespace BankAccountServices.Mappers
{
	public class OperationProfile : Profile
	{
		public OperationProfile()
		{
		
			// AccountOperation -> DTO
			CreateMap<AccountOperation, OperationResponseDTO>()
				.ForMember(dest => dest.AccountType,
						   opt => opt.MapFrom(src => src.BankAccount.AccountType))
				.ForMember(dest => dest.NameCustomer,
			   opt => opt.MapFrom(src => src.BankAccount.Customer.Name));

			CreateMap<OperationCreatedDTO, AccountOperation>(); //DTO CRATED => Entity

		}
	}
}
