using Job.Domain.Entities.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.Infrastructure.Configurations.Motoboy;

[ExcludeFromCodeCoverage]
public class MotoboyConfiguration : IEntityTypeConfiguration<MotoboyEntity>
{
    public void Configure(EntityTypeBuilder<MotoboyEntity> builder)
    {
        builder.ToTable("Motoboy");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Cnpj)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(x => x.Cnh)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.CnhImage)
            .HasMaxLength(500);

        builder.HasIndex(x => x.Cnpj)
            .IsUnique();

        builder.HasIndex(x => x.Cnh)
            .IsUnique();
    }
}