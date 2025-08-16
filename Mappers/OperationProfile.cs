using AutoMapper;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.DTOs.Operation;
using BankAccountServices.Entities;

namespace BankAccountServices.Mappers
{
	public class OperationProfile:Profile
	{
		public OperationProfile()
		{
			CreateMap<AccountOperation, OperationResponseDTO>(); // Entity -> DTO RESPONSE
			CreateMap<OperationCreatedDTO, AccountOperation>(); //DTO CRATED => Entity
		}
	}
}
