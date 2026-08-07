
using AutoMapper;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.Repositories.Interfaces;

namespace BankAccountServices.Features.Customers.Queries.GetCustomersQueryHandler;

public class GetCustomersQueryHandler(
ICustomerRepository repository,
IMapper mapper)
{
    public List<CustomerResponseDTO> Handle()
    {

        var customers = repository.GettAllCustomer();
        return mapper.Map<List<CustomerResponseDTO>>(customers);


    }
}