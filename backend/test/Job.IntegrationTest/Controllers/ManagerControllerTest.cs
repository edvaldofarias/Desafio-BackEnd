using Job.Application.Commands.Manager;
using Job.IntegrationTest.Fixtures;

namespace Job.IntegrationTest.Controllers;

[Collection("Database")]
[Trait("Integration", "Manager")]
public class ManagerControllerTest(SetupFactory factory) : IClassFixture<SetupFactory>
{
    private readonly Faker _faker = new();

    [SkippableFact]
    public async Task AuthenticationManagerCommand_WithUnknownEmail_ShouldReturnBadRequest()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient();
        var content = new AuthenticationManagerCommand(_faker.Person.Email, _faker.Internet.Password());

        var response = await client.PostAsJsonAsync("/manager/authentication", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [SkippableFact]
    public async Task AuthenticationManagerCommand_WithSeededCredentials_ShouldReturnOk()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient();
        var content = new AuthenticationManagerCommand("job@job.com", "mudar@123");

        var response = await client.PostAsJsonAsync("/manager/authentication", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [SkippableFact]
    public async Task AuthenticationManagerCommand_WithGarbage_ShouldReturnBadRequest()
    {
        Skip.IfNot(factory.Db.IsAvailable, factory.Db.UnavailableReason);

        var client = factory.CreateClient();
        var content = new AuthenticationManagerCommand(_faker.Random.AlphaNumeric(10), _faker.Random.AlphaNumeric(5));

        var response = await client.PostAsJsonAsync("/manager/authentication", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
