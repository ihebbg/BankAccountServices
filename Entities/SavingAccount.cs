using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountServices.Entities
{
	public class SavingAccount:BankAccount
	{
		[Column("interestrate")]
		public double InterestRate { get; set; }
	}
}
