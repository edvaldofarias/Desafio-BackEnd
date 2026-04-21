using System.Reflection;
using FluentValidation;
using Job.Application.Commands.Manager;
using Job.Application.Commands.Manager.Validations;
using Job.Application.Messaging;
using Job.Application.Repositories;
using Job.Application.Services;
using Job.Infrastructure.Context;
using Job.Infrastructure.Messaging;
using Job.Infrastructure.Repositories;
using Job.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;

namespace Job.WebApi.Infrastructure;

public static class DependencyInjectService
{
    public static void AddDependencyInject(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<JobContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.Configure<LocalFileStorageOptions>(configuration.GetSection("Storage"));
        services.AddSingleton<IFileStorageService, LocalFileStorageService>();

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
        services.AddHostedService<MotoCreatedConsumer>();

        var applicationAssembly = Assembly.Load("Job.Application");

        services.RegisterRepository();
        services.RegisterValidation();
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(applicationAssembly));
    }

    private static void RegisterRepository(this IServiceCollection services)
    {
        services.AddScoped<IManagerRepository, ManagerRepository>();
        services.AddScoped<IMotoboyRepository, MotoboyRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IMotoRepository, MotoRepository>();
        services.AddScoped<IMotoNotificationRepository, MotoNotificationRepository>();
    }

    private static void RegisterValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AuthenticationManagerCommand>, AuthenticationManagerValidation>();
    }
}