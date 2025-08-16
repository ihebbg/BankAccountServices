using BankAccountServices.Entities;

namespace BankAccountServices.Repositories.Interfaces
{
    public interface ICustomerRepository

    {
        IEnumerable<Customer> GettAllCustomer();
        Customer GetCustomer(long ididCustomer);
        long AddCustomer(Customer customer);
        void UpdateCustomer(Customer newCustomer,long idCustomer);
        void DeleteCustomer(Customer customer);
        bool CustomerEmaiLExist(string email);



    }
}
