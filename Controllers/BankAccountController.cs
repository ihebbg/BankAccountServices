using BankAccountServices.DTOs;
using Microsoft.AspNetCore.Mvc;
using BankAccountServices.Services.Interfaces;
using BankAccountServices.DTOs.BankAccount.SavingBankAccount;
using BankAccountServices.DTOs.BankAccount.CurrentBankAccount;
using BankAccountServices.DTOs.BankAccount;
using BankAccountServices.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankAccountServices.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BankAccountController(IBankAccountService bankAccountService) : ControllerBase


	{
		private readonly IBankAccountService _bankAccountService = bankAccountService;

		/// <summary>
		/// Add new bank account saving
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[HttpPost("saving/add")]
		public ActionResult<Retour> AddBankAccountSaving(SavingBankAccountCreateDTO input)
		{

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return Ok(_bankAccountService.AddBankAccountSaving(input));


		}

		/// <summary>
		/// Add new bank account current
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[HttpPost("current/add")]
		public ActionResult<Retour> AddBankAccountCurrent(CurrentBankAccountCreateDTO input)
		{

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(_bankAccountService.AddBankAccountCurrent(input));


		}
		/// <summary>
		/// GET Bank Account
		/// </summary>
		/// <param name="idBankAccount"></param>
		/// <returns></returns>

		[HttpGet("{idBankAccount}")]
		public ActionResult<BankAccountResponseDTO> GetBankAccount(int idBankAccount)
		{
			return Ok(_bankAccountService.GetBankAccountById(idBankAccount));
		}
		/// <summary>
		/// Delete bankAccount
		/// </summary>
		/// <param name="idBankAccount"></param>
		/// <returns></returns>
		[HttpDelete("supprimer/{idBankAccount}")]
		public ActionResult<Retour> DeleteBankAccount(int idBankAccount)
		{

			return Ok(_bankAccountService.DeleteBankAccount(idBankAccount));


		}
		/// <summary>
		/// Get all bank account
		/// </summary>
		/// <returns></returns>
		[HttpGet("liste")]
		public ActionResult<List<BankAccountResponseDTO>> GetBankAccount()
		{

			return Ok(_bankAccountService.GetAllBankAccounts());


		}
		/// <summary>
		/// Get bank account by status
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		[HttpGet("liste/status/{status}")]
		public ActionResult<List<BankAccountResponseDTO>> GetBankAccountByStatus(AccountStatus status)
		{

			return Ok(_bankAccountService.GetBankAccountByStatus(status));


		}
		/// <summary>
		/// Get bank account by type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[HttpGet("liste/type/{type}")]
		public ActionResult<List<BankAccountResponseDTO>> GetBankAccountByStatus(string type)
		{

			return Ok(_bankAccountService.GetBankAccountByType(type));


		}


		/// <summary>
		/// Get list bank account with pagination
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet("liste/paginated")]
		public IActionResult GetBankAccountPaginated(int page = 1, int pageSize = 10)
		{

			return Ok(_bankAccountService.GetPaginatedBankAccount(page, pageSize));



		}
	}
}
