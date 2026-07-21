using BankAccountServices.DTOs;
using BankAccountServices.DTOs.S3;
using BankAccountServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankAccountServices.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class S3Controller(IS3StorageService s3StorageService) : ControllerBase
	{
		private readonly IS3StorageService _s3StorageService = s3StorageService;

		[HttpPost("upload")]
		[Consumes("multipart/form-data")]
		[RequestSizeLimit(50 * 1024 * 1024)]
		public async Task<ActionResult<S3FileResponseDTO>> Upload(IFormFile file, [FromQuery] string? folder, CancellationToken cancellationToken)
		{
			return Ok(await _s3StorageService.UploadFileAsync(file, folder, cancellationToken));
		}

		[HttpGet("download")]
		public async Task<IActionResult> Download([FromQuery] string key, CancellationToken cancellationToken)
		{
			var file = await _s3StorageService.DownloadFileAsync(key, cancellationToken);
			return File(file.Content, file.ContentType, file.FileName);
		}

		[HttpGet("presigned-url")]
		public async Task<ActionResult<S3PresignedUrlResponseDTO>> GetPresignedUrl([FromQuery] string key, [FromQuery] int? expirationMinutes)
		{
			return Ok(await _s3StorageService.GetPresignedUrlAsync(key, expirationMinutes));
		}

		[HttpDelete]
		public async Task<ActionResult<Retour>> Delete([FromQuery] string key, CancellationToken cancellationToken)
		{
			await _s3StorageService.DeleteFileAsync(key, cancellationToken);
			return Ok(new Retour
			{
				Code = CodeRetour.Ok,
				Message = "Fichier supprimé de S3"
			});
		}
	}
}
