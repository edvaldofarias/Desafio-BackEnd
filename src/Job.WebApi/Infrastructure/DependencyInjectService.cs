using FluentValidation;
using Job.Domain.Commands.Rent;
using Job.Domain.Commands.Rent.Validations;
using Job.Domain.Commands.User.Manager;
using Job.Domain.Commands.User.Manager.Validations;
using Job.Domain.Commands.User.Motoboy;
using Job.Domain.Commands.User.Motoboy.Validations;
using Job.Domain.Repositories;
using Job.Domain.Services;
using Job.Domain.Services.Interfaces;
using Job.Domain.UseCases.Moto.Create.Commands;
using Job.Domain.UseCases.Moto.Create.Commands.Validations;
using Job.Domain.UseCases.Moto.Delete.Commands;
using Job.Domain.UseCases.Moto.Delete.Commands.Validations;
using Job.Domain.UseCases.Moto.Update.Commands;
using Job.Domain.UseCases.Moto.Update.Commands.Validations;
using Job.Infrastructure.Context;
using Job.Infrastructure.Repositories;
using Job.WebApi.Services;
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

        services.RegisterService();
        services.RegisterRepository();
        services.RegisterValidation();
        services.AddTransient<TokenService>();
    }

    private static void RegisterRepository(this IServiceCollection services)
    {
        services.AddScoped<IManagerRepository, ManagerRepository>();
        services.AddScoped<IMotoboyRepository, MotoboyRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IMotoRepository, MotoRepository>();
    }

    private static void RegisterService(this IServiceCollection services)
    {
        services.AddScoped<IMotoboyService, MotoboyService>();
        services.AddScoped<IRentService, RentalService>();
        services.AddScoped<IMotoService, MotoService>();
        services.AddScoped<IManagerService, ManagerService>();
    }

    private static void RegisterValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateMotoCommand>, CreateMotoValidation>();
        services.AddScoped<IValidator<UpdateMotoCommand>, UpdateMotoValidation>();
        services.AddScoped<IValidator<DeleteMotoCommand>, DeleteMotoValidation>();

        services.AddScoped<IValidator<CancelRentalCommand>, CancelRentalValidation>();
        services.AddScoped<IValidator<CreateRentalCommand>, CreateRentalValidation>();

        services.AddScoped<IValidator<AuthenticationManagerCommand>, AuthenticationManagerValidation>();

        services.AddScoped<IValidator<AuthenticationMotoboyCommand>, AuthenticationMotoboyValidation>();
        services.AddScoped<IValidator<CreateMotoboyCommand>, CreateMotoboyValidation>();
        services.AddScoped<IValidator<UploadCnhMotoboyCommand>, UploadCnhMotoboyValidation>();
    }
}