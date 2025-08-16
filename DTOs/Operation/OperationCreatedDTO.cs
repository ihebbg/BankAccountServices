using System.ComponentModel.DataAnnotations;
using BankAccountServices.Enums;

namespace BankAccountServices.DTOs.Operation
{
	public class OperationCreatedDTO
	{
		[Range(0.01, double.MaxValue)]

		public double Amount { get; set; }


		public OperationsType Type { get; set; }


		public long IdBankAccount { get; set; }
	}
}
