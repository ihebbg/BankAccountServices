using System.Text;

namespace BankAccountServices.Configuration;

public sealed class JwtSettings
{
	public required string Key { get; init; }
	public required string Issuer { get; init; }
	public required string Audience { get; init; }

	public static JwtSettings FromConfiguration(IConfiguration configuration)
	{
		var section = configuration.GetRequiredSection("Jwt");

		var settings = new JwtSettings
		{
			Key = GetRequiredValue(section, "Key"),
			Issuer = GetRequiredValue(section, "Issuer"),
			Audience = GetRequiredValue(section, "Audience")
		};

		if (Encoding.UTF8.GetByteCount(settings.Key) < 32)
		{
			throw new InvalidOperationException("Configuration 'Jwt:Key' must be at least 32 bytes for HS256 signing.");
		}

		return settings;
	}

	private static string GetRequiredValue(IConfiguration section, string key)
	{
		var value = section[key];
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new InvalidOperationException(
				$"Missing required configuration value 'Jwt:{key}'. Set it in appsettings or with the 'Jwt__{key}' environment variable.");
		}

		return value;
	}
}
