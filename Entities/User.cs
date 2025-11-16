using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountServices.Entities
{
	[Table("ba_user")]

	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("id_user")]
		public long IdUser { get; set; }

		[Required]
		[StringLength(100)]

		[Column("login")]

		public required string Login { get; set; }

		[Required]
		[StringLength(500)]
		[Column("password")]

		public required string Password { get; set; }

		[Required]
		[Column("id_role")]
		public required long IdRole { get; set; }

		[ForeignKey("IdRole")]
		public required Role Role { get; set; }
		public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

	}
}
