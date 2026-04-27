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
                Name: faker.Person.FullName,
                Cnpj: faker.Company.Cnpj(),
                Password: faker.Internet.Password(8),
                DateBirth: faker.Person.DateOfBirth,
                Cnh: "77058710884",
                TypeCnh: faker.PickRandom("A", "B", "A+B"),
                Identifier: faker.Random.AlphaNumeric(8)
            ));
    }

    public static Faker<CreateMotoboyCommand> Invalid()
    {
        return new Faker<CreateMotoboyCommand>()
            .CustomInstantiator(faker => new CreateMotoboyCommand(
                Name: string.Empty,
                Cnpj: faker.Random.AlphaNumeric(5),
                Password: string.Empty,
                DateBirth: DateTime.Now,
                Cnh: faker.Random.AlphaNumeric(5),
                TypeCnh: "Z"
            ));
    }
}
