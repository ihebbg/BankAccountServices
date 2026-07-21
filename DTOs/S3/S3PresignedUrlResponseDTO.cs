namespace BankAccountServices.DTOs.S3;

public sealed class S3PresignedUrlResponseDTO
{
	public required string Key { get; init; }
	public required string Url { get; init; }
	public DateTime ExpiresAtUtc { get; init; }
}
