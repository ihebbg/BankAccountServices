using BankAccountServices.DTOs;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.Features.Customers.Commands.AddCustomer;
using BankAccountServices.Features.Customers.Commands.DeleteCustomerHandler;
using BankAccountServices.Features.Customers.Commands.UpdateCustomerHandler;
using BankAccountServices.Features.Customers.Queries.GetCustomerById;
using BankAccountServices.Features.Customers.Queries.GetCustomersQueryHandler;

using Microsoft.AspNetCore.Mvc;
namespace BankAccountServices.Controllers
{
	//[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class CustomerController(AddCustomerHandler addHandler,
    UpdateCustomerHandler updateHandler,
    DeleteCustomerHandler deleteHandler,
    GetCustomerByIdQueryHandler getByIdHandler,
    GetCustomersQueryHandler getAllHandler) : ControllerBase
	{

		/// <summary>
		/// Add new customer
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[HttpPost("add")]
		public ActionResult<Retour> AddCustomer(CustomerCreateDTO input)
		{

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(addHandler.Handle(input));



		}
		/// <summary>
		/// Get customer by id
		/// </summary>
		/// <param name="idCustomer"></param>
		/// <returns></returns>
		[HttpGet("{idCustomer}")]
		public ActionResult<CustomerResponseDTO> GetCustomerByID(int idCustomer)
		{

			return Ok(getByIdHandler.Handle(idCustomer));
		}
		/// <summary>
		/// Get all customers
		/// </summary>
		/// <returns></returns>

		[HttpGet("liste")]
		public ActionResult<List<CustomerResponseDTO>> GetCustomers()
		{
			//var idUsdddefdr = User.FindFirst("jwtLogin");
			return Ok(getAllHandler.Handle());

		}
		// [HttpGet("liste/paginated")]
		// public ActionResult<CustomerResponseDTO> GetPaginatedCustomers(int page = 1, int pageSize = 10)
		// {

		// 	return Ok(_customerService.GetPaginatedCustomers(page, pageSize));


		// }

		/// <summary>
		/// Delete Customer
		/// </summary>
		/// <param name="idCustomer"></param>
		/// <returns></returns>
		[HttpDelete("delete/{idCustomer}")]
		public ActionResult<Retour> DeleteCustomer(long idCustomer)
		{


			return Ok(deleteHandler.Handle(idCustomer));


		}
		/// <summary>
		/// Update customer
		/// </summary>
		/// <param name="customer"></param>
		/// <param name="idCustomer"></param>
		/// <returns></returns>
		[HttpPut("modifier/{idCustomer}")]
		public ActionResult<Retour> UpdateCustomer(CustomerUpdateDTO customer, long idCustomer)
		{
			return Ok(updateHandler.Handle(customer, idCustomer));
		}
	}
}
