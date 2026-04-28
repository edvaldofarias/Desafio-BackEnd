using Job.IntegrationTest.Fixtures;
using System.Text.Json;

namespace Job.IntegrationTest.Controllers;

[Collection("Database")]
[Trait("Integration", "Moto")]
public class MotoControllerTest(SetupFactory factory) : IClassFixture<SetupFactory>
{
    private readonly Faker _faker = new();

    [SkippableFact]
    public async Task Create_WithValidPayload_ReturnsCreated()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient().WithAdminAuth();
        var identifier = $"moto-{Guid.NewGuid():N}";
        var payload = new
        {
            identificador = identifier,
            ano = 2024,
            modelo = "Mottu Sport",
            placa = $"AAA{_faker.Random.Number(1000, 9999)}",
        };

        var response = await client.PostAsJsonAsync("/motos", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [SkippableFact]
    public async Task Create_WithInvalidYear_ReturnsBadRequestWithMensagem()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient().WithAdminAuth();
        var payload = new
        {
            identificador = $"moto-{Guid.NewGuid():N}",
            ano = 1800,
            modelo = "X",
            placa = "ZZZ0000",
        };

        var response = await client.PostAsJsonAsync("/motos", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.TryGetProperty("mensagem", out _).Should().BeTrue();
    }

    [SkippableFact]
    public async Task GetById_WhenExists_ReturnsMoto()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient().WithAdminAuth();
        var identifier = $"moto-{Guid.NewGuid():N}";
        var plate = $"BBB{_faker.Random.Number(1000, 9999)}";
        await client.PostAsJsonAsync("/motos", new
        {
            identificador = identifier,
            ano = 2024,
            modelo = "Mottu Sport",
            placa = plate,
        });

        var response = await client.GetAsync($"/motos/{identifier}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("identificador").GetString().Should().Be(identifier);
        doc.RootElement.GetProperty("placa").GetString().Should().Be(plate);
    }

    [SkippableFact]
    public async Task GetById_WhenMissing_ReturnsNotFoundWithMensagem()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient().WithAdminAuth();
        var response = await client.GetAsync($"/motos/{Guid.NewGuid():N}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("mensagem").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task GetAll_WhenMotoboyAuthenticated_ReturnsOk()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var admin = factory.CreateClient().WithAdminAuth();
        await admin.PostAsJsonAsync("/motos", new
        {
            identificador = $"moto-{Guid.NewGuid():N}",
            ano = 2024,
            modelo = "Mottu Sport",
            placa = $"MTO{_faker.Random.Number(1000, 9999)}",
        });

        var motoboy = factory.CreateClient().WithMotoboyAuth();
        var response = await motoboy.GetAsync("/motos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.ValueKind.Should().Be(JsonValueKind.Array);
    }

    [SkippableFact]
    public async Task UpdatePlate_WhenValid_ReturnsSuccess()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient().WithAdminAuth();
        var identifier = $"moto-{Guid.NewGuid():N}";
        await client.PostAsJsonAsync("/motos", new
        {
            identificador = identifier,
            ano = 2024,
            modelo = "Mottu Sport",
            placa = $"CCC{_faker.Random.Number(1000, 9999)}",
        });

        var newPlate = $"DDD{_faker.Random.Number(1000, 9999)}";
        var response = await client.PutAsJsonAsync($"/motos/{identifier}/placa", new { placa = newPlate });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        var get = await client.GetAsync($"/motos/{identifier}");
        var body = await get.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("placa").GetString().Should().Be(newPlate);
    }

    [SkippableFact]
    public async Task Delete_WhenNoRental_ReturnsSuccess()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient().WithAdminAuth();
        var identifier = $"moto-{Guid.NewGuid():N}";
        await client.PostAsJsonAsync("/motos", new
        {
            identificador = identifier,
            ano = 2024,
            modelo = "Mottu Sport",
            placa = $"EEE{_faker.Random.Number(1000, 9999)}",
        });

        var response = await client.DeleteAsync($"/motos/{identifier}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }
}
