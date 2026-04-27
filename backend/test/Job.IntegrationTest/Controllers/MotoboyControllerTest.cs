using Bogus.Extensions.Brazil;
using Job.IntegrationTest.Fixtures;
using System.Text.Json;

namespace Job.IntegrationTest.Controllers;

[Collection("Database")]
[Trait("Integration", "Motoboy")]
public class MotoboyControllerTest(SetupFactory factory) : IClassFixture<SetupFactory>
{
    private readonly Faker _faker = new();

    // 1x1 png base64 (válido)
    private const string PngBase64 =
        "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkAAIAAAoAAv/lxKUAAAAASUVORK5CYII=";

    [SkippableFact]
    public async Task Create_WithValidPayload_ReturnsCreated()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient();
        var payload = new
        {
            identificador = $"entregador-{Guid.NewGuid():N}",
            nome = _faker.Person.FullName,
            cnpj = _faker.Company.Cnpj(false),
            data_nascimento = new DateTime(1990, 1, 1),
            numero_cnh = TestData.GenerateValidCnh(),
            tipo_cnh = "A",
            imagem_cnh = (string?)null,
        };

        var response = await client.PostAsJsonAsync("/entregadores", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [SkippableFact]
    public async Task Create_WithInvalidCnh_ReturnsBadRequestWithMensagem()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient();
        var payload = new
        {
            identificador = $"entregador-{Guid.NewGuid():N}",
            nome = _faker.Person.FullName,
            cnpj = _faker.Company.Cnpj(false),
            data_nascimento = new DateTime(1990, 1, 1),
            numero_cnh = "abc",
            tipo_cnh = "Z",
            imagem_cnh = (string?)null,
        };

        var response = await client.PostAsJsonAsync("/entregadores", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.TryGetProperty("mensagem", out _).Should().BeTrue();
    }

    [SkippableFact]
    public async Task UploadCnh_WithValidPng_ReturnsSuccess()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient();
        var identifier = $"entregador-{Guid.NewGuid():N}";
        await client.PostAsJsonAsync("/entregadores", new
        {
            identificador = identifier,
            nome = _faker.Person.FullName,
            cnpj = _faker.Company.Cnpj(false),
            data_nascimento = new DateTime(1990, 1, 1),
            numero_cnh = TestData.GenerateValidCnh(),
            tipo_cnh = "A",
            imagem_cnh = (string?)null,
        });

        var response = await client.PostAsJsonAsync(
            $"/entregadores/{identifier}/cnh",
            new { imagem_cnh = PngBase64 });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.NoContent);
    }
}
