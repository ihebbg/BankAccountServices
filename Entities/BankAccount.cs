using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BankAccountServices.Enums;

namespace BankAccountServices.Entities
{
	[Table("ba_bank_account")]
	public class BankAccount
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("id_bank_account")]

		public long IdBankAccount { get; set; }
		[Required]
		[Column("balance")]
		public double Balance { get; set; }

		[Required]
		[Column("created")]
		public DateTime Created { get; set; }

		[Required]
		[StringLength(50)]

		[Column("currency")]
		public string Currency { get; set; }

		[Required]
		[Column("status")]

		public AccountStatus Status { get; set; }


		[Required]
		[Column("id_customer")]
		public long IdCustomer { get; set; }

		[ForeignKey("IdCustomer")]
		public required Customer Customer { get; set; } 
		[NotMapped]
		public string AccountType => this switch
		{
			SavingAccount => "Saving",
			CurrentAccount => "Current",
			_ => ""
		};
		public ICollection<AccountOperation> Operations { get; set; } = new List<AccountOperation>();

	}
}
