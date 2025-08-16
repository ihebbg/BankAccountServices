
namespace BankAccountServices.DTOs.Operation
{
	public class OperationResponseDTO
	{
		public long IdAccountOperation { get; set; }
		public DateTime Date { get; set; }

		public double Amount { get; set; }


		public required string Type { get; set; }


		public long IdBankAccount { get; set; }
	}
}
