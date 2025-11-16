using System.ComponentModel.DataAnnotations;

namespace BankAccountServices.DTOs.Customer
{
    public class CustomerCreateDTO
    {
        public required string Name { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
    }
}
