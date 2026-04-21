using Job.Domain.Entities.Rental;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.Infrastructure.Configurations.Rental;

[ExcludeFromCodeCoverage]
public class RentalConfiguration : IEntityTypeConfiguration<RentalEntity>
{
    public void Configure(EntityTypeBuilder<RentalEntity> builder)
    {
        builder.ToTable("Rental");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Identifier)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IdMoto).IsRequired();
        builder.Property(x => x.IdMotoboy).IsRequired();
        builder.Property(x => x.Plan).IsRequired();
        builder.Property(x => x.DailyValue).HasPrecision(10, 2);
        builder.Property(x => x.Value).HasPrecision(10, 2);
        builder.Property(x => x.Fine).HasPrecision(10, 2);

        builder.HasIndex(x => x.Identifier).IsUnique();
        builder.HasIndex(x => x.IdMoto);
    }
}