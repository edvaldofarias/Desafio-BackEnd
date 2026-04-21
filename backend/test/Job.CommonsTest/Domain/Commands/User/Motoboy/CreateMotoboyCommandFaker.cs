using Bogus;
using Bogus.Extensions.Brazil;
using Job.Application.Commands.Motoboy;

namespace Job.Commons.Domain.Commands.User.Motoboy;

public static class CreateMotoboyCommandFaker
{
    public static Faker<CreateMotoboyCommand> Default()
    {
        return new Faker<CreateMotoboyCommand>()
            .CustomInstantiator(faker => new CreateMotoboyCommand(
                Identifier: faker.Random.AlphaNumeric(8),
                Name: faker.Person.FullName,
                Cnpj: faker.Company.Cnpj(),
                DateBirth: faker.Person.DateOfBirth,
                Cnh: "77058710884",
                TypeCnh: faker.PickRandom("A", "B", "A+B")
            ));
    }

    public static Faker<CreateMotoboyCommand> Invalid()
    {
        return new Faker<CreateMotoboyCommand>()
            .CustomInstantiator(faker => new CreateMotoboyCommand(
                Identifier: string.Empty,
                Name: string.Empty,
                Cnpj: faker.Random.AlphaNumeric(5),
                DateBirth: DateTime.Now,
                Cnh: faker.Random.AlphaNumeric(5),
                TypeCnh: "Z"
            ));
    }
}
