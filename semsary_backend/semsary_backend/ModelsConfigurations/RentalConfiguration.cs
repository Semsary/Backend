using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using semsary_backend.Models;

namespace semsary_backend.ModelsConfigurations
{
    public class RentalConfiguration : IEntityTypeConfiguration<Rental>
    {
        public void Configure(EntityTypeBuilder<Rental> builder)
        {
            builder.HasKey(r => r.RentalId);

            builder.Property(r => r.RentalId)
                .ValueGeneratedOnAdd();


            builder.HasOne(r => r.RentalUnit)
                .WithMany(r => r.Rentals)
                .HasForeignKey(r => r.RentalUnitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Complaint)
                .WithOne(r => r.Rental)
                .HasForeignKey<Complaint>(r => r.RentalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }


}
