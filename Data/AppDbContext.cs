using BankAccountServices.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankAccountServices.Data
{
	public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
	{
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<BankAccount>()
			.HasDiscriminator<string>("account_type")
			.HasValue<SavingAccount>("Saving")
			.HasValue<CurrentAccount>("Current");
			base.OnModelCreating(modelBuilder);
		}


		public DbSet<Customer> Customers { get; set; }
		public DbSet<BankAccount> BankAccounts { get; set; }
		public DbSet<AccountOperation> AccountOperations { get; set; }

		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }

	}
}
