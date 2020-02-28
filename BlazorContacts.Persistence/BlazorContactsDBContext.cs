using BlazorContacts.Application;
using BlazorContacts.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorContacts.Persistence
{
	public static class CommonColumns
	{
		public const string CreatedAt = "CreatedAt";
		public const string UpdatedAt = "UpdatedAt";
	}

	public class BlazorContactsDBContext : DbContext, IBlazorContactsDBContext
	{
		public BlazorContactsDBContext(DbContextOptions<BlazorContactsDBContext> options) : base(options) { }

		public DbSet<Contact> Contacts { get; set; }

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			HandleAuditFields();
			return base.SaveChangesAsync(cancellationToken);
		}

		private void HandleAuditFields()
		{
			ChangeTracker.DetectChanges();

			foreach (var entry in ChangeTracker.Entries())
			{
				var properties = entry.Properties.ToList();

				var createdAt = properties?.FirstOrDefault(p => p.Metadata.Name == CommonColumns.CreatedAt);
				var updatedAt = properties?.FirstOrDefault(p => p.Metadata.Name == CommonColumns.UpdatedAt);

				var currentDateTime = DateTime.UtcNow;

				if (entry.State == EntityState.Added)
				{
					if (createdAt != null)
					{
						var valueOfCreated = entry.Property(CommonColumns.CreatedAt).CurrentValue;
						if (valueOfCreated is DateTime && (DateTime)valueOfCreated == DateTime.MinValue)
							entry.Property(CommonColumns.CreatedAt).CurrentValue = currentDateTime;
					}

					if (updatedAt != null)
						entry.Property(CommonColumns.UpdatedAt).CurrentValue = currentDateTime;
				}

				if (entry.State == EntityState.Modified)
				{
					if (updatedAt != null)
						entry.Property(CommonColumns.UpdatedAt).CurrentValue = currentDateTime;
				}
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlazorContactsDBContext).Assembly);
		}

		public override DatabaseFacade Database => base.Database;
	}
}
