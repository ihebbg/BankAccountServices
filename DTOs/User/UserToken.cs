namespace BankAccountServices.DTOs.User
{
	public class UserToken
	{
		public required string Login { get; set; }
		public int IdRole { get; set; }
		public required string Role { get; set; }

	}
}
