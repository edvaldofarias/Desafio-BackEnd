using Job.Domain.Entities.Notification;
using Job.Infrastructure.Context;
using Job.IntegrationTest.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Job.IntegrationTest.Controllers;

[Collection("Database")]
[Trait("Integration", "Messaging")]
public class MotoMessagingTest(SetupFactory factory) : IClassFixture<SetupFactory>
{
    private readonly Faker _faker = new();

    private JobContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<JobContext>()
            .UseNpgsql(factory.Db.ConnectionString)
            .Options;
        return new JobContext(options);
    }

    private async Task<MotoNotificationEntity?> WaitNotificationAsync(string plate, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            await using var ctx = CreateContext();
            var found = await ctx.Set<MotoNotificationEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Plate == plate);
            if (found is not null) return found;
            await Task.Delay(250);
        }
        return null;
    }

    [SkippableFact]
    public async Task PublishMotoCreated_When2024_PersistsNotification()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);
        Skip.IfNot(factory.Rabbit.IsAvailable, factory.Rabbit.UnavailableReason);

        var client = factory.CreateClient().WithAdminAuth();
        var plate = $"RBT{_faker.Random.Number(10000, 99999)}";
        var payload = new
        {
            identificador = $"moto-{Guid.NewGuid():N}",
            ano = 2024,
            modelo = "Mottu Sport",
            placa = plate,
        };

        var response = await client.PostAsJsonAsync("/motos", payload);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var notification = await WaitNotificationAsync(plate, TimeSpan.FromSeconds(15));

        notification.Should().NotBeNull("o consumer deve persistir notificação para motos do ano 2024");
        notification!.Year.Should().Be(2024);
        notification.Plate.Should().Be(plate);
    }

    [SkippableFact]
    public async Task PublishMotoCreated_WhenNot2024_IsIgnoredByConsumer()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);
        Skip.IfNot(factory.Rabbit.IsAvailable, factory.Rabbit.UnavailableReason);

        var client = factory.CreateClient().WithAdminAuth();
        var plate = $"IGN{_faker.Random.Number(10000, 99999)}";
        var payload = new
        {
            identificador = $"moto-{Guid.NewGuid():N}",
            ano = 2023,
            modelo = "Mottu Pop",
            placa = plate,
        };

        var response = await client.PostAsJsonAsync("/motos", payload);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Aguarda janela suficiente para o consumer processar e descartar
        var notification = await WaitNotificationAsync(plate, TimeSpan.FromSeconds(5));

        notification.Should().BeNull("eventos com Year != 2024 devem ser ignorados pelo consumer");
    }
}
