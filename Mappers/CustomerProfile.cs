using AutoMapper;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.Entities;

namespace BankAccountServices.Mappers
{
	public class CustomerProfile:Profile
	{
		public CustomerProfile()
		{
			CreateMap<CustomerCreateDTO, Customer>(); // DTO CREATE-> Entity
			CreateMap<Customer, CustomerResponseDTO>(); // Entity -> DTO RESPONSE
			CreateMap<CustomerUpdateDTO, Customer>(); //DTO UPDATE ->Entity
		}
	}
}
