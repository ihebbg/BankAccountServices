namespace BankAccountServices.DTOs.S3;

public sealed class S3DownloadFileDTO
{
	public required Stream Content { get; init; }
	public required string ContentType { get; init; }
	public required string FileName { get; init; }
}
