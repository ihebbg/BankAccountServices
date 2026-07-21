using BankAccountServices.DTOs.S3;

namespace BankAccountServices.Services.Interfaces
{
	public interface IS3StorageService
	{
		Task<S3FileResponseDTO> UploadFileAsync(IFormFile file, string? folder, CancellationToken cancellationToken = default);
		Task<S3DownloadFileDTO> DownloadFileAsync(string key, CancellationToken cancellationToken = default);
		Task<S3PresignedUrlResponseDTO> GetPresignedUrlAsync(string key, int? expirationMinutes = null);
		Task DeleteFileAsync(string key, CancellationToken cancellationToken = default);
	}
}
