using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using semsary_backend.Models;

namespace semsary_backend.ModelsConfigurations
{
    public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {
            builder.HasKey(c => c.ComplaintId);

            builder.Property(c => c.ComplaintId)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.SubmittingDate)
                .IsRequired();
            
            builder.Property(c => c.ComplaintDetails)
                .IsRequired();

            builder.Property(r => r.RentalId)
                .IsRequired();

        }
    }
}
