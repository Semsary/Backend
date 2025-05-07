using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using semsary_backend.Models;

namespace semsary_backend.ModelsConfigurations
{
    public class HouseInspectionConfiguration : IEntityTypeConfiguration<HouseInspection>
    {
        public void Configure(EntityTypeBuilder<HouseInspection> builder)
        {
            builder.HasKey(h => h.HouseInspectionId);

            builder.Property(h => h.HouseInspectionId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(h => h.InspectionDate)
                .IsRequired();

            builder.HasMany( h => h.HouseImages)
                .WithOne(r => r.HouseInspection)
                .HasForeignKey(r => r.HouseInspectionId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
