using BankAccountServices.Data;
using BankAccountServices.Entities;
using BankAccountServices.Repositories.Interfaces;

namespace BankAccountServices.Repositories
{
    public class CustomerRepository: ICustomerRepository
	{
		private readonly AppDbContext _appDbContext;
		public CustomerRepository( AppDbContext appDbContext) {
			_appDbContext = appDbContext;
		}

		public long AddCustomer(Customer customer)
		{
			_appDbContext.Customers.Add(customer);
			_appDbContext.SaveChanges();
			return customer.IdCustomer;
			
		}
		public bool CustomerEmaiLExist(string email)
		{
			return _appDbContext.Customers.Any(c => c.Email.ToLower().Trim() == email);
		}
		public void DeleteCustomer(Customer customer)
		{
			_appDbContext.Customers.Remove(customer);
			_appDbContext.SaveChanges();

		}

		public Customer GetCustomer(long idCustomer)
		{
			return _appDbContext.Customers.Find(idCustomer)!;
		}

		public IEnumerable<Customer> GettAllCustomer()
		{
			return _appDbContext.Customers.ToList();
		}

		public void UpdateCustomer(Customer newCustomer, long idCustomer)
		{
			var existing = _appDbContext.Customers.Find(idCustomer);
			if (existing != null)
			{
				existing.Name = newCustomer.Name;
				_appDbContext.SaveChanges();
			}

		}

	
	}
}
