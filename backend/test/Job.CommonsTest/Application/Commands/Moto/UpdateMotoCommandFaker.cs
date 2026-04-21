using Bogus;
using Job.Application.Commands.Moto;

namespace Job.Commons.Application.Commands.Moto;

public static class UpdateMotoCommandFaker
{
    public static Faker<UpdateMotoCommand> Default()
    {
        return new Faker<UpdateMotoCommand>()
            .CustomInstantiator(faker => new UpdateMotoCommand(
                Identifier: faker.Random.AlphaNumeric(8),
                Plate: "AAA5F55"
            ));
    }

    public static Faker<UpdateMotoCommand> Empty()
    {
        return new Faker<UpdateMotoCommand>()
            .CustomInstantiator(_ => new UpdateMotoCommand(
                Identifier: string.Empty,
                Plate: string.Empty
            ));
    }

    public static Faker<UpdateMotoCommand> Invalid()
    {
        return new Faker<UpdateMotoCommand>()
            .CustomInstantiator(faker => new UpdateMotoCommand(
                Identifier: string.Empty,
                Plate: faker.Random.AlphaNumeric(1)
            ));
    }
}
