using BankAccountServices.DTOs;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.Entities;

namespace BankAccountServices.Services.Interfaces
{
    public interface ICustomerService
    {
        Retour AddCustomer(CustomerCreateDTO customer);
        bool CustomerEmaiLExist(string email);

        CustomerResponseDTO GetCustomerByID(long id);
        List<CustomerResponseDTO> GetCustomers();
        Retour DeleteCustomer(long idCustomer);
        Retour UpdateCustomer(CustomerUpdateDTO customer, long idCustomer);
        List<CustomerResponseDTO> GetPaginatedCustomers(int page, int pageSize);


	}
}
