using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountServices.Entities
{
	[Table("ba_refresh_token")]
	public class RefreshToken
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("id_refresh_token")]
		public long IdRefreshToken {  get; set; }
		[Column("token")]

		public string Token {  get; set; }
		[Column("date_expiration")]

		public DateTime DateExpiration { get; set; }
		[Column("date_creation")]

		public DateTime DateCreation { get; set; }
		[Column("id_revoked")]

		public bool IsRevoked { get; set; }

		[Column("id_user")] // clé étrangère vers User
		public long IdUser { get; set; }

		// Navigation property
		[ForeignKey("IdUser")] // dit à EF que User est lié à IdUser
		public User User { get; set; }

	}
}
