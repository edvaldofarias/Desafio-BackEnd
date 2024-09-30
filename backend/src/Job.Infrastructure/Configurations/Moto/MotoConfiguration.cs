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
    }
}