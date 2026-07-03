using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankAccountServices.Data;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	private const string DesignTimeConnectionString =
		"server=localhost;port=3306;database=bankaccountdb;user=root;password=design-time-only";

	public AppDbContext CreateDbContext(string[] args)
	{
		var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: true)
			.AddJsonFile($"appsettings.{environment}.json", optional: true)
			.AddEnvironmentVariables()
			.Build();

		var connectionString = configuration.GetConnectionString("DBConnection") ?? DesignTimeConnectionString;

		var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
		optionsBuilder.UseMySql(
			connectionString,
			new MySqlServerVersion(new Version(8, 0, 36)),
			mySqlOptions => mySqlOptions.EnableRetryOnFailure());

		return new AppDbContext(optionsBuilder.Options);
	}
}
