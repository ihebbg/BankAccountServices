using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountServices.Entities
{
	[Table("ba_role")]

	public class Role
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("id_role")]
		public required long IdRole {  get; set; }

		[Column("role_name")]
		[Required]

		public required string RoleName { get; set; }

	}
}
