using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using semsary_backend.Models;

namespace semsary_backend.ModelsConfigurations
{
    public class RentalUnitConfiguration : IEntityTypeConfiguration<RentalUnit>
    {
        public void Configure(EntityTypeBuilder<RentalUnit> builder)
        {
            builder.HasKey(r => r.RentalUnitId);

            builder.Property(r => r.RentalUnitId)
                .IsRequired();

            builder.Property(r => r.AdvertisementId)
                .IsRequired();
        }
    }
}
