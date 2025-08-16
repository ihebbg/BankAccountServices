using AutoMapper;
using BankAccountServices.DTOs.BankAccount;
using BankAccountServices.DTOs.BankAccount.CurrentBankAccount;
using BankAccountServices.DTOs.BankAccount.SavingBankAccount;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankAccountServices.Mappers
{
    public class BankAccountProfile:Profile
	{
		public BankAccountProfile() {

			CreateMap<SavingBankAccountCreateDTO, SavingAccount>(); // DTO BANK ACCOUNT SAVING CREATE-> Entity
			CreateMap<CurrentBankAccountCreateDTO, CurrentAccount>(); // DTO BANK ACCOUNT SAVING CREATE-> Entity
			CreateMap<BankAccount, BankAccountResponseDTO>().ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType));//BANK ACCOUNT ENTITY =>Bank Account response DTO
			CreateMap<BankAccountResponseDTO, BankAccount>();


		}
	}
}
