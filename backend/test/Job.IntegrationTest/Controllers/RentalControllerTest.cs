using Bogus.Extensions.Brazil;
using Job.IntegrationTest.Fixtures;
using System.Text.Json;

namespace Job.IntegrationTest.Controllers;

[Collection("Database")]
[Trait("Integration", "Rental")]
public class RentalControllerTest(SetupFactory factory) : IClassFixture<SetupFactory>
{
    private readonly Faker _faker = new();

    private async Task<string> CreateMotoAsync(HttpClient client)
    {
        var identifier = $"moto-{Guid.NewGuid():N}";
        var resp = await client.PostAsJsonAsync("/motos", new
        {
            identificador = identifier,
            ano = 2024,
            modelo = "Mottu Sport",
            placa = $"ZZZ{_faker.Random.Number(1000, 9999)}",
        });
        resp.EnsureSuccessStatusCode();
        return identifier;
    }

    private async Task<string> CreateMotoboyAsync(HttpClient client, string typeCnh = "A")
    {
        var identifier = $"entregador-{Guid.NewGuid():N}";
        var resp = await client.PostAsJsonAsync("/entregadores", new
        {
            identificador = identifier,
            nome = _faker.Person.FullName,
            cnpj = _faker.Company.Cnpj(false),
            data_nascimento = new DateTime(1990, 1, 1),
            numero_cnh = TestData.GenerateValidCnh(),
            tipo_cnh = typeCnh,
            imagem_cnh = (string?)null,
        });
        resp.EnsureSuccessStatusCode();
        return identifier;
    }

    [SkippableFact]
    public async Task Create_WithCnhA_ReturnsCreated()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient();
        var moto = await CreateMotoAsync(client);
        var motoboy = await CreateMotoboyAsync(client, "A");
        var rentalId = $"locacao-{Guid.NewGuid():N}";
        var start = DateTime.UtcNow.Date.AddDays(1);

        var payload = new
        {
            identificador = rentalId,
            entregador_id = motoboy,
            moto_id = moto,
            data_inicio = start,
            data_termino = start.AddDays(7),
            data_previsao_termino = start.AddDays(7),
            plano = 7,
        };

        var response = await client.PostAsJsonAsync("/locacao", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [SkippableFact]
    public async Task Create_WithCnhB_ReturnsBadRequestWithMensagem()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient();
        var moto = await CreateMotoAsync(client);
        var motoboy = await CreateMotoboyAsync(client, "B");
        var start = DateTime.UtcNow.Date.AddDays(1);

        var payload = new
        {
            identificador = $"locacao-{Guid.NewGuid():N}",
            entregador_id = motoboy,
            moto_id = moto,
            data_inicio = start,
            data_termino = start.AddDays(7),
            data_previsao_termino = start.AddDays(7),
            plano = 7,
        };

        var response = await client.PostAsJsonAsync("/locacao", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.TryGetProperty("mensagem", out _).Should().BeTrue();
    }

    [SkippableFact]
    public async Task GetById_WhenExists_ReturnsRental()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient();
        var moto = await CreateMotoAsync(client);
        var motoboy = await CreateMotoboyAsync(client, "A");
        var rentalId = $"locacao-{Guid.NewGuid():N}";
        var start = DateTime.UtcNow.Date.AddDays(1);

        await client.PostAsJsonAsync("/locacao", new
        {
            identificador = rentalId,
            entregador_id = motoboy,
            moto_id = moto,
            data_inicio = start,
            data_termino = start.AddDays(7),
            data_previsao_termino = start.AddDays(7),
            plano = 7,
        });

        var response = await client.GetAsync($"/locacao/{rentalId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("identificador").GetString().Should().Be(rentalId);
    }

    [SkippableFact]
    public async Task Return_OnPlanEndDate_ReturnsSuccess()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient();
        var moto = await CreateMotoAsync(client);
        var motoboy = await CreateMotoboyAsync(client, "A");
        var rentalId = $"locacao-{Guid.NewGuid():N}";
        var start = DateTime.UtcNow.Date.AddDays(1);
        var preview = start.AddDays(7);

        await client.PostAsJsonAsync("/locacao", new
        {
            identificador = rentalId,
            entregador_id = motoboy,
            moto_id = moto,
            data_inicio = start,
            data_termino = preview,
            data_previsao_termino = preview,
            plano = 7,
        });

        var response = await client.PutAsJsonAsync(
            $"/locacao/{rentalId}/devolucao",
            new { data_devolucao = preview });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }
}
