using AutoMapper;
using BankAccountServices.DTOs;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.Entities;
using BankAccountServices.Repositories.Interfaces;
using BankAccountServices.Services.Interfaces;
namespace BankAccountServices.Services
{
	public class CustomerService(ICustomerRepository repository, IMapper mapper) : ICustomerService

	{
		private readonly ICustomerRepository _repository = repository;
		private readonly IMapper _mapper = mapper;

		public Retour AddCustomer(CustomerCreateDTO input)
		{
			Retour r = new();

			if (CustomerEmaiLExist(input.Email))
			{
				r.Code = CodeRetour.Ko;
				r.Message = "Email exist";
				return r;

			}
			var customer = _mapper.Map<Customer>(input);

			var id = _repository.AddCustomer(customer);
			r.Code = CodeRetour.Ok;
			r.Message = "Customer Added success2";
			r.ID = id;
			return r;

		}
		public CustomerResponseDTO GetCustomerByID(long idCustomer)
		{

			if (idCustomer <= 0)
			{
				throw new ArgumentException("Le type doit être un nombre positif.", nameof(idCustomer));
			}

			var customer = _repository.GetCustomer(idCustomer);
			if (customer == null)
				throw new KeyNotFoundException($"Aucune customer trouvée avec l'ID {idCustomer}.");
			return _mapper.Map<CustomerResponseDTO>(customer);



		}
		public List<CustomerResponseDTO> GetCustomers()
		{

			var customers = _repository.GettAllCustomer();
			return _mapper.Map<List<CustomerResponseDTO>>(customers);


		}


		public bool CustomerEmaiLExist(string email)
		{

			return _repository.CustomerEmaiLExist(email.ToLower().Trim());

		}

		public Retour DeleteCustomer(long idCustomer)
		{
			Retour r = new();

			var customer = GetCustomerByID(idCustomer);
			if (customer != null)
			{
				var customerEntity = _mapper.Map<Customer>(customer);
				_repository.DeleteCustomer(customerEntity);
				r.Code = CodeRetour.Ok;
				r.Message = "Customer Deleted";
			}
			return r;




		}

		public Retour UpdateCustomer(CustomerUpdateDTO customer, long idCustomer)
		{
			Retour r = new();

			var existing = _repository.GetCustomer(idCustomer);
			if (existing == null)
			{
				r.Code = CodeRetour.Ko;
				r.Message = "Customer not found";
				return r;
			}
			var c = _mapper.Map<Customer>(customer);
			_repository.UpdateCustomer(c, idCustomer);
			r.Code = CodeRetour.Ok;
			r.Message = "Customer updated";
			return r;

		}

		public List<CustomerResponseDTO> GetPaginatedCustomers(int page, int pageSize)
		{

			var customers = _repository.GettAllCustomer().Skip((page - 1) * pageSize)
		.Take(pageSize);

			return _mapper.Map<List<CustomerResponseDTO>>(customers);
		}
	}
}
