using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using semsary_backend.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

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

            builder.Property(r => r.VerifiedBy)
                .IsRequired(false);

            builder.Property(r => r.ComplaintReview)
                .IsRequired(false);

            builder.HasOne(c => c.CustomerService)
               .WithMany(c => c.Complaints)
               .HasForeignKey(c => c.VerifiedBy);

            builder.HasOne(c => c.Tenant)
               .WithMany(c => c.Complaints)
               .HasForeignKey(c => c.SubmittedBy);

        }

    }
}
