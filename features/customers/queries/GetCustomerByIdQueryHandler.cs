using AutoMapper;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.Helpers;
using BankAccountServices.Repositories.Interfaces;

namespace BankAccountServices.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler(
    ICustomerRepository repository,
    IMapper mapper)
{
    public CustomerResponseDTO Handle(int customerId)
    {
        PositiveNumberValidationHelper.Validate(customerId, nameof(customerId));

        var customer = repository.GetCustomer(customerId) ?? throw new KeyNotFoundException("Customer introuvable.");
        return mapper.Map<CustomerResponseDTO>(customer);
    }




}
