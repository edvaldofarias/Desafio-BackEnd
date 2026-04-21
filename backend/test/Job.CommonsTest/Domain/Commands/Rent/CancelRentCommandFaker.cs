using Bogus;
using Job.Application.Commands.Rental;

namespace Job.Commons.Domain.Commands.Rent;

public static class CancelRentCommandFaker
{
    public static Faker<CancelRentalCommand> Default()
    {
        return new Faker<CancelRentalCommand>()
            .CustomInstantiator(faker => new CancelRentalCommand(
                Identifier: faker.Random.AlphaNumeric(8),
                DateReturn: faker.Date.Future()
            ));
    }

    public static Faker<CancelRentalCommand> Empty()
    {
        return new Faker<CancelRentalCommand>()
            .CustomInstantiator(_ => new CancelRentalCommand(
                Identifier: string.Empty,
                DateReturn: DateTime.MinValue
            ));
    }

    public static Faker<CancelRentalCommand> Invalid()
    {
        return new Faker<CancelRentalCommand>()
            .CustomInstantiator(_ => new CancelRentalCommand(
                Identifier: string.Empty,
                DateReturn: DateTime.MinValue
            ));
    }
}
