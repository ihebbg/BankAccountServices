
using BankAccountServices.DTOs.BankAccount;

namespace BankAccountServices.DTOs.Operation
{
	public class OperationResponseDTO
	{
		public long IdAccountOperation { get; set; }
		public DateTime Date { get; set; }

		public double Amount { get; set; }


		public required string Type { get; set; }


		public string AccountType { get; set; }
		public string NameCustomer { get; set; }
	}
}
