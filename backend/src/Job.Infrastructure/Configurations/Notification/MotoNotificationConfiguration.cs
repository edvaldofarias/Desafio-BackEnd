using Job.Domain.Entities.Notification;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.Infrastructure.Configurations.Notification;

[ExcludeFromCodeCoverage]
public class MotoNotificationConfiguration : IEntityTypeConfiguration<MotoNotificationEntity>
{
    public void Configure(EntityTypeBuilder<MotoNotificationEntity> builder)
    {
        builder.ToTable("MotoNotification");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.MotoId)
            .IsRequired();

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Plate)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.OccurredAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.MotoId);
    }
}
