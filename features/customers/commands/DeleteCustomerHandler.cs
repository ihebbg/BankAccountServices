
using BankAccountServices.DTOs;
using BankAccountServices.Helpers;
using BankAccountServices.Repositories.Interfaces;

namespace BankAccountServices.Features.Customers.Commands.DeleteCustomerHandler;

public class DeleteCustomerHandler(
ICustomerRepository repository)
{

    public Retour Handle(long idCustomer)
    {
        PositiveNumberValidationHelper.Validate(idCustomer, nameof(idCustomer));
        var customer = repository.GetCustomer(idCustomer) ?? throw new KeyNotFoundException(
                $"Customer {idCustomer} introuvable.");
        repository.DeleteCustomer(customer);

        return new Retour
        {
            Code = CodeRetour.Ok,
            Message = "Customer supprimé"
        };

    }
}
