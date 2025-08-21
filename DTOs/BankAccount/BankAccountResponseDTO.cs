using BankAccountServices.DTOs.Customer;
using BankAccountServices.Enums;

namespace BankAccountServices.DTOs.BankAccount
{
	public class BankAccountResponseDTO
	{
		public long IdBankAccount {  get; set; }	
		public double Balance { get; set; }
		public DateTime Created { get; set; }
		public required string Currency { get; set; }
		public AccountStatus Status { get; set; }
		public required string AccountType { get; set; }
		public required string NameCustomer { get; set; }
	}
}
