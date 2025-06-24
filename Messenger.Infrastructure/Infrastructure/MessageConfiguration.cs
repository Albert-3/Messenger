using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Messenger.Infrastructure.Infrastructure
{
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(m => m.SenderId).IsRequired();
            builder.HasOne(r => r.Sender).WithMany(r => r.Messages).HasForeignKey(r => r.SenderId);
            builder.Property(m => m.RecipientId).IsRequired();
        }
    }
}
