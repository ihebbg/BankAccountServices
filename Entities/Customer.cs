using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;

namespace BankAccountServices.Entities
{
	[Table("ba_customer")]
	public class Customer
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("id_customer")]
		public long IdCustomer { get; set; }

		[Required]
		[StringLength(50)]
		[Column("name")]

		public required string Name { get; set; }

		[Required]
		[StringLength(100)]
		[EmailAddress]
		[Column("email")] 
		public required string Email { get; set; }

		public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

	}
}

