using System.Reflection;
using FluentValidation;
using Job.Application.Commands.Manager;
using Job.Application.Commands.Manager.Validations;
using Job.Application.Repositories;
using Job.Infrastructure.Context;
using Job.Infrastructure.Repositories;
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

        var applicationAssembly = Assembly.Load("Job.Infrastructure");

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
    }

    private static void RegisterValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AuthenticationManagerCommand>, AuthenticationManagerValidation>();
    }
}