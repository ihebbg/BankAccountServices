namespace BankAccountServices.Configuration;

public sealed record S3Settings
{
	public const string SectionName = "S3";

	public string BucketName { get; init; } = string.Empty;
	public string Region { get; init; } = string.Empty;
	public string Prefix { get; init; } = string.Empty;
	public string PublicBaseUrl { get; init; } = string.Empty;
	public string ServiceUrl { get; init; } = string.Empty;
	public bool ForcePathStyle { get; init; }
	public int PreSignedUrlExpirationMinutes { get; init; } = 15;

	public static S3Settings FromConfiguration(IConfiguration configuration)
	{
		var section = configuration.GetSection(SectionName);
		var settings = section.Get<S3Settings>() ?? new S3Settings();
		var region = FirstNotEmpty(
			settings.Region,
			configuration["AWS:Region"],
			Environment.GetEnvironmentVariable("AWS_REGION"),
			Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION"));

		settings = settings with
		{
			BucketName = settings.BucketName.Trim(),
			Region = region ?? string.Empty,
			Prefix = settings.Prefix.Trim().Trim('/'),
			PublicBaseUrl = settings.PublicBaseUrl.Trim().TrimEnd('/'),
			ServiceUrl = settings.ServiceUrl.Trim().TrimEnd('/')
		};

		if (string.IsNullOrWhiteSpace(settings.BucketName))
		{
			throw new InvalidOperationException(
				"Missing required configuration value 'S3:BucketName'. Set it in appsettings or with the 'S3__BucketName' environment variable.");
		}

		if (string.IsNullOrWhiteSpace(settings.Region) && string.IsNullOrWhiteSpace(settings.ServiceUrl))
		{
			throw new InvalidOperationException(
				"Missing S3 region. Set 'S3:Region', 'AWS:Region', 'AWS_REGION', or configure 'S3:ServiceUrl' for a custom S3-compatible endpoint.");
		}

		if (settings.PreSignedUrlExpirationMinutes <= 0)
		{
			throw new InvalidOperationException("Configuration 'S3:PreSignedUrlExpirationMinutes' must be greater than zero.");
		}

		return settings;
	}

	private static string? FirstNotEmpty(params string?[] values)
	{
		return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim();
	}
}
