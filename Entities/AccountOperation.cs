using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BankAccountServices.Enums;

namespace BankAccountServices.Entities
{
	[Table("ba_acount_operation")]

	public class AccountOperation
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("id_account_operation")]
		public long IdAccountOperation { get; set; }

		[Required]
		[Column("date")]
		public DateTime Date { get; set; }

		[Required]
		[Column("amount")]
		public double Amount { get; set; }

		[Required]
		[Column("type")]
		public OperationsType Type { get; set; }

		[Required]
		[Column("id_bank_account")]
		public long IdBankAccount { get; set; }

		[ForeignKey("IdBankAccount")]
		public BankAccount BankAccount { get; set; }
	}
}
