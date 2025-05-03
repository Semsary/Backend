using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using semsary_backend.Models;

namespace semsary_backend.ModelsConfigurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.MessageId);

            builder.Property(m => m.MessageId)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.SenderUsername)
                .IsRequired();

            builder.Property(m => m.ReceiverUsername)
                .IsRequired();

            builder.Property(m => m.Content)
                .IsRequired();
        }
    }

}
