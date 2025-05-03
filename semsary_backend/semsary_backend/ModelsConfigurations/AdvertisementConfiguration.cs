using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using semsary_backend.Models;

namespace semsary_backend.ModelsConfigurations
{
    public class AdvertisementConfiguration : IEntityTypeConfiguration<Advertisement>
    {
        public void Configure(EntityTypeBuilder<Advertisement> builder)
        {
            builder.HasKey(a => a.AdvertisementId);

            builder.Property(a => a.AdvertisementId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(a => a.HouseId)
                .IsRequired();

            builder.Property(a => a.PublishDate)
                .IsRequired();

            builder.HasMany(h => h.RentalUnits)
                .WithOne(r => r.Advertisement)
                .HasForeignKey(r => r.AdvertisementId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
