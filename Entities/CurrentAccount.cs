using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountServices.Entities
{
	public class CurrentAccount: BankAccount
	{
		[Column("overdraft")]
		public double OverDraft { get; set; }
	}
}
