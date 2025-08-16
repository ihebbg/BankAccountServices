using System.ComponentModel.DataAnnotations;

namespace BankAccountServices.DTOs.Customer
{
    public class CustomerCreateDTO
    {
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
