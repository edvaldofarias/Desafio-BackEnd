using Job.Domain.Entities.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.Infrastructure.Configurations.Manager;

[ExcludeFromCodeCoverage]
public class ManagerConfiguration : IEntityTypeConfiguration<ManagerEntity>
{
    private const int WorkFactor = 12;
    public void Configure(EntityTypeBuilder<ManagerEntity> builder)
    {
        builder.ToTable("Manager");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        var password = BCrypt.Net.BCrypt.HashPassword("mudar@123", WorkFactor);
        const string email = "job@job.com";
        builder.HasData(new ManagerEntity(email, password));
    }
}