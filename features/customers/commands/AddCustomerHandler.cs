using AutoMapper;
using BankAccountServices.DTOs;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.Entities;
using BankAccountServices.Repositories.Interfaces;

namespace BankAccountServices.Features.Customers.Commands.AddCustomer;

public class AddCustomerHandler(
    ICustomerRepository repository,
    IMapper mapper)
{
    	public Retour Handle(CustomerCreateDTO input)
		{
			       var email = input.Email.Trim().ToLowerInvariant();

        if (repository.CustomerEmaiLExist(email))
        {
            throw new InvalidOperationException(
                "Email already exists.");
        }

      	var customer = mapper.Map<Customer>(input);
        var customerId = repository.AddCustomer(customer);

        return new Retour
        {
            Code = CodeRetour.Ok,
            Message = "Customer added successfully",
            ID = customerId
        };

		}


}