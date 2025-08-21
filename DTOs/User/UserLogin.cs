namespace BankAccountServices.DTOs.User
{
	public class UserLogin
	{
		public int IdUser { get; set; }
		public required string Login { get; set; }
		public int IdRole { get; set; }
		public required string Role { get; set; }

	}
}
