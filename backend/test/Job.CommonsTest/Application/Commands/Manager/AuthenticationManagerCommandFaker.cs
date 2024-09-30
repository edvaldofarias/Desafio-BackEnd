using Bogus;
using Job.Application.Commands.Manager;

namespace Job.Commons.Application.Commands.Manager;

public static class AuthenticationManagerCommandFaker
{
    public static Faker<AuthenticationManagerCommand> Default(string? password = null)
    {
        return new Faker<AuthenticationManagerCommand>()
            .CustomInstantiator(faker => new AuthenticationManagerCommand(
                faker.Internet.Email(),
                password ?? faker.Internet.Password()
            ));
    }

    public static Faker<AuthenticationManagerCommand> Invalid()
    {
        return new Faker<AuthenticationManagerCommand>()
            .CustomInstantiator(faker => new AuthenticationManagerCommand(
                faker.Random.AlphaNumeric(5),
                faker.Lorem.Letter(5)
            ));
    }
}