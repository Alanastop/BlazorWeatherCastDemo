using BlazorContacts.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorContacts.Persistence.Configurations
{
	class ContactConfiguration : IEntityTypeConfiguration<Contact>
	{
		public void Configure(EntityTypeBuilder<Contact> builder)
		{
			builder.HasKey(e => e.Id);

			builder.Property(e => e.Id).HasColumnName("ContactId");

			builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);

			builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);

			builder.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(50);

			builder.Property(e => e.CreatedAt).IsRequired().HasColumnType("Datetime");

			//builder.HasOne(d => d.Document)
			//	.WithMany(p => p.AgentHistoryItems)
			//	.HasForeignKey(a => a.DocumentId)
			//	.HasConstraintName("FK_AgentHistoryItems_Documents");

			//builder.HasIndex(e => e.Status).HasName("IX_AgentHistoryItem_Status");
		}
	}
}
