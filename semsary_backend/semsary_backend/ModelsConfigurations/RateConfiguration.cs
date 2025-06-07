using Microsoft.EntityFrameworkCore;
using semsary_backend.Models;

namespace semsary_backend.ModelsConfigurations
{
    public class RateConfiguration : IEntityTypeConfiguration<Rate>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Rate> builder)
        {
            builder.HasKey(r => r.RateId);

            builder.Property(r => r.RateId)
                .IsRequired();

            builder.Property(r => r.RateId)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.HouseId)
                .IsRequired();

            builder.Property(r => r.TenantUsername)
                .IsRequired();

            builder.Property(r => r.RateDate)
                .IsRequired();

            builder.Property(r => r.StarsNumber)
                .IsRequired();

        }
    }
}
