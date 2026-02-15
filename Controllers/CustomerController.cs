using BankAccountServices.DTOs;
using BankAccountServices.DTOs.Customer;
using BankAccountServices.Entities;
using BankAccountServices.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BankAccountServices.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class CustomerController(ICustomerService customerService) : ControllerBase
	{
		private readonly ICustomerService _customerService = customerService;

		/// <summary>
		/// Add new customer
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[HttpPost("add")]
		public ActionResult<Retour> AddCustomer(CustomerCreateDTO input)
		{

			if (ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(_customerService.AddCustomer(input));



		}
		/// <summary>
		/// Get customer by id
		/// </summary>
		/// <param name="idCustomer"></param>
		/// <returns></returns>
		[HttpGet("{idCustomer}")]
		public ActionResult<CustomerResponseDTO> GetCustomerByID(long idCustomer)
		{

			return Ok(_customerService.GetCustomerByID(idCustomer));
		}
		/// <summary>
		/// Get all customers
		/// </summary>
		/// <returns></returns>
		//[Authorize(Roles = "Administrateur")]

		[HttpGet("liste")]
		public ActionResult<List<CustomerResponseDTO>> GetCustomers()
		{
			var idUsdddefr = User.FindFirst("jwtLogin");
			return Ok(_customerService.GetCustomers());

		}
		[HttpGet("liste/paginated")]
		public ActionResult<CustomerResponseDTO> GetPaginatedCustomers(int page = 1, int pageSize = 10)
		{

			return Ok(_customerService.GetPaginatedCustomers(page, pageSize));


		}

		/// <summary>
		/// Delete Customer
		/// </summary>
		/// <param name="idCustomer"></param>
		/// <returns></returns>
		[HttpDelete("delete/{idCustomer}")]
		public ActionResult<Retour> DeleteCustomer(long idCustomer)
		{


			return Ok(_customerService.DeleteCustomer(idCustomer));


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

			var existCustomer = _customerService.GetCustomerByID(idCustomer);
			if (existCustomer == null)
			{
				return NotFound();
			}
			return Ok(_customerService.UpdateCustomer(customer, idCustomer));
		}
	}
}
