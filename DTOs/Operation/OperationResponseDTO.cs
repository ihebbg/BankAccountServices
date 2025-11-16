
using BankAccountServices.DTOs.BankAccount;

namespace BankAccountServices.DTOs.Operation
{
	public class OperationResponseDTO
	{
		public long IdAccountOperation { get; set; }
		public DateTime Date { get; set; }

		public double Amount { get; set; }


		public required string Type { get; set; }


		public required string  AccountType { get; set; }
		public required string NameCustomer { get; set; }
	}
}
