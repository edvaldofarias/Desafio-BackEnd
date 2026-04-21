using Job.Domain.Entities.Moto;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.Infrastructure.Configurations.Moto;

[ExcludeFromCodeCoverage]
public class MotoConfiguration : IEntityTypeConfiguration<MotoEntity>
{
    public void Configure(EntityTypeBuilder<MotoEntity> builder)
    {
        builder.ToTable("Moto");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Identifier)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Plate)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(x => x.Identifier)
            .IsUnique();

        builder.HasIndex(x => x.Plate)
            .IsUnique();
    }
}