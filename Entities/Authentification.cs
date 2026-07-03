using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankAccountServices.Entities
{
	[Table("ba_authentification")]

	public class Authentification
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("id_authentification")]
		public long IdAuthentification { get; set; }

		[Required]
		[Column("login")]
		public DateTime Login { get; set; }
	}
}
