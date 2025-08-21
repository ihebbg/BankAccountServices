namespace BankAccountServices.DTOs.Customer
{
	public class CustomerResponseDTO
	{
		public long IdCustomer {  get; set; }
		public required string Name { get; set; }
		public required string Email {  get; set; }	

	}
}
