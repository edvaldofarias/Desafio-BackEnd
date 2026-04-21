using Bogus;
using Job.Application.Commands.Moto;

namespace Job.Commons.Application.Commands.Moto;

public static class CreateMotoCommandFaker
{
    public static Faker<CreateMotoCommand> Default()
    {
        return new Faker<CreateMotoCommand>()
            .CustomInstantiator(faker => new CreateMotoCommand(
                Identifier: faker.Random.AlphaNumeric(8),
                Year: faker.Random.Int(1900, 2050),
                Model: faker.Vehicle.Model(),
                Plate: "AAA5F55"
            ));
    }

    public static Faker<CreateMotoCommand> Empty()
    {
        return new Faker<CreateMotoCommand>()
            .CustomInstantiator(_ => new CreateMotoCommand(
                Identifier: string.Empty,
                Year: 0,
                Model: string.Empty,
                Plate: string.Empty
            ));
    }

    public static Faker<CreateMotoCommand> Invalid()
    {
        return new Faker<CreateMotoCommand>()
            .CustomInstantiator(faker => new CreateMotoCommand(
                Identifier: string.Empty,
                Year: faker.Random.Int(0, 1899),
                Model: string.Empty,
                Plate: faker.Random.AlphaNumeric(1)
            ));
    }
}
