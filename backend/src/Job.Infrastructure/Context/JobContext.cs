using Job.Domain.Entities.Moto;
using Job.Domain.Entities.Notification;
using Job.Domain.Entities.Rental;
using Job.Domain.Entities.User;
using Job.Infrastructure.Conversions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Job.Infrastructure.Context;

[ExcludeFromCodeCoverage]
public sealed class JobContext : DbContext
{
    public JobContext(DbContextOptions<JobContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter, DateOnlyComparer>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<RentalEntity> Rents => Set<RentalEntity>();
    public DbSet<MotoboyEntity> Motoboys => Set<MotoboyEntity>();
    public DbSet<MotoEntity> Motos => Set<MotoEntity>();
    public DbSet<ManagerEntity> Managers => Set<ManagerEntity>();
    public DbSet<MotoNotificationEntity> MotoNotifications => Set<MotoNotificationEntity>();
}