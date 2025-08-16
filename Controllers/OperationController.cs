using BankAccountServices.DTOs;
using BankAccountServices.DTOs.Operation;
using BankAccountServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankAccountServices.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OperationController(IOperationService operationService) : ControllerBase
	{
		private readonly IOperationService _operationService=operationService;
		/// <summary>
		/// Get all operations
		/// </summary>
		/// <returns></returns>
		[HttpGet("liste")]
		public ActionResult<List<OperationResponseDTO>> GetOperations()
		{
			return Ok(_operationService.GetAllOperatiosn());

		}
		/// <summary>
		/// Get operation by id
		/// </summary>
		/// <param name="idOperation"></param>
		/// <returns></returns>

		[HttpGet("{idOperation}")]
		public ActionResult<OperationResponseDTO>GetOperation(long idOperation)
		{
		
			return Ok(_operationService.GetOerationById(idOperation));
		}
		/// <summary>
		/// Credit operation
		/// </summary>
		/// <param name="idBankAccount"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		[HttpPost("{idBankAccount}/credit")]
		public ActionResult<Retour> Credit(long idBankAccount, double amount)
		{
			return Ok(_operationService.CrediBankAccount(idBankAccount, amount));	
		}
		/// <summary>
		/// Debit operation
		/// </summary>
		/// <param name="idBankAccount"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		[HttpPost("{idBankAccount}/debit")]
		public ActionResult<Retour> Debit(long idBankAccount, double amount)
		{
			return Ok(_operationService.DebitBankAccount(idBankAccount, amount));

		}
	}
}
