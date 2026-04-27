using Bogus;
using Job.Application.Commands.Moto;

namespace Job.Commons.Application.Commands.Moto;

public static class CreateMotoCommandFaker
{
    public static Faker<CreateMotoCommand> Default()
    {
        return new Faker<CreateMotoCommand>()
            .CustomInstantiator(faker => new CreateMotoCommand(
                Year: faker.Random.Int(1901, 2050),
                Model: $"{faker.Vehicle.Model()}-{faker.Random.AlphaNumeric(3)}",
                Plate: "AAA5F55",
                Identifier: faker.Random.AlphaNumeric(8)
            ));
    }

    public static Faker<CreateMotoCommand> Empty()
    {
        return new Faker<CreateMotoCommand>()
            .CustomInstantiator(_ => new CreateMotoCommand(
                Year: 0,
                Model: string.Empty,
                Plate: string.Empty,
                Identifier: string.Empty
            ));
    }

    public static Faker<CreateMotoCommand> Invalid()
    {
        return new Faker<CreateMotoCommand>()
            .CustomInstantiator(faker => new CreateMotoCommand(
                Year: faker.Random.Int(0, 1899),
                Model: string.Empty,
                Plate: faker.Random.AlphaNumeric(1),
                Identifier: string.Empty
            ));
    }
}
