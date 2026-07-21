namespace BankAccountServices.DTOs.S3;

public sealed class S3FileResponseDTO
{
	public required string Key { get; init; }
	public required string FileName { get; init; }
	public required string ContentType { get; init; }
	public long Size { get; init; }
	public string? Url { get; init; }
}
