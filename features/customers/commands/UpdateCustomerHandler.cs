
using BankAccountServices.DTOs;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.Entities;
using BankAccountServices.Helpers;
using BankAccountServices.Repositories.Interfaces;

namespace BankAccountServices.Features.Customers.Commands.UpdateCustomerHandler;

public class UpdateCustomerHandler(
ICustomerRepository repository)
{

    public Retour Handle(CustomerUpdateDTO customer, long idCustomer)
    {

        PositiveNumberValidationHelper.Validate(idCustomer, nameof(idCustomer));

        var existingCustomer =
            repository.GetCustomer(idCustomer);

        if (existingCustomer is null)
        {
            throw new KeyNotFoundException(
                $"Aucun customer trouvé avec l'ID {idCustomer}.");
        }

        var updatedCustomer = new Customer
        {
            Name = customer.Name.Trim(),
            Email = existingCustomer.Email
        };

        repository.UpdateCustomer(
            updatedCustomer,
            idCustomer);

        return new Retour
        {
            Code = CodeRetour.Ok,
            Message = "Customer updated"
        };
    }
}
