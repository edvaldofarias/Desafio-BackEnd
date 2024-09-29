using Bogus;
using Bogus.Extensions.Brazil;
using Job.Application.Commands.Rental;
using Job.Domain.Enums;

namespace Job.Commons.Domain.Commands.Rent;

public static class CreateRentCommandFaker
{
    public static Faker<CreateRentalCommand> Default()
    {
        return new Faker<CreateRentalCommand>()
            .CustomInstantiator(faker => new CreateRentalCommand(
                faker.Random.Guid(),
                faker.Date.Future(),
                faker.PickRandom<EPlan>()
            )
            {
                Cnpj = faker.Company.Cnpj()
            });
    }

    public static Faker<CreateRentalCommand> Empty()
    {
        return new Faker<CreateRentalCommand>()
            .CustomInstantiator(_ => new CreateRentalCommand(
                Guid.Empty,
                DateTime.MinValue,
                0
            ));
    }

    public static Faker<CreateRentalCommand> Invalid()
    {
        return new Faker<CreateRentalCommand>()
            .CustomInstantiator(faker => new CreateRentalCommand(
                Guid.Empty,
                faker.Date.Past(),
                0
            ));
    }
}