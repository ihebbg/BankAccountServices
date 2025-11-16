using System.ComponentModel.DataAnnotations;
using BankAccountServices.Enums;

namespace BankAccountServices.DTOs.BankAccount
{
	public class BankAccountCreateDTO
	{
		[Required]
		public double Balance { get; set; }

		[Required]
		public required string Currency { get; set; }

		[Required]
		public AccountStatus Status { get; set; }

		[Required]
		public long IdCustomer { get; set; }
	}
}
