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

    }
}