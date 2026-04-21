using Bogus;
using Job.Application.Commands.Moto;

namespace Job.Commons.Application.Commands.Moto;

public static class UpdateMotoCommandFaker
{
    public static Faker<UpdateMotoCommand> Default()
    {
        return new Faker<UpdateMotoCommand>()
            .CustomInstantiator(faker => new UpdateMotoCommand(
                faker.Random.Guid(),
                "AAA5F55"
            ));
    }

    public static Faker<UpdateMotoCommand> Empty()
    {
        return new Faker<UpdateMotoCommand>()
            .CustomInstantiator(_ => new UpdateMotoCommand(
                Guid.Empty,
                string.Empty
            ));
    }

    public static Faker<UpdateMotoCommand> Invalid()
    {
        return new Faker<UpdateMotoCommand>()
            .CustomInstantiator(faker => new UpdateMotoCommand(
                Guid.Empty,
                faker.Random.AlphaNumeric(1)
            ));
    }
}