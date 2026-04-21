using Bogus;
using Job.Application.Commands.Rental;
using Job.Domain.Enums;

namespace Job.Commons.Domain.Commands.Rent;

public static class CreateRentCommandFaker
{
    public static Faker<CreateRentalCommand> Default()
    {
        return new Faker<CreateRentalCommand>()
            .CustomInstantiator(faker =>
            {
                var start = faker.Date.Future();
                var preview = start.AddDays(7);
                var end = preview.AddDays(1);
                return new CreateRentalCommand(
                    Identifier: faker.Random.AlphaNumeric(8),
                    MotoboyIdentifier: faker.Random.AlphaNumeric(8),
                    MotoIdentifier: faker.Random.AlphaNumeric(8),
                    DateStart: start,
                    DateEnd: end,
                    DatePreview: preview,
                    Plan: EPlan.Sete);
            });
    }

    public static Faker<CreateRentalCommand> Empty()
    {
        return new Faker<CreateRentalCommand>()
            .CustomInstantiator(_ => new CreateRentalCommand(
                Identifier: string.Empty,
                MotoboyIdentifier: string.Empty,
                MotoIdentifier: string.Empty,
                DateStart: DateTime.MinValue,
                DateEnd: DateTime.MinValue,
                DatePreview: DateTime.MinValue,
                Plan: 0));
    }

    public static Faker<CreateRentalCommand> Invalid()
    {
        return new Faker<CreateRentalCommand>()
            .CustomInstantiator(faker => new CreateRentalCommand(
                Identifier: string.Empty,
                MotoboyIdentifier: string.Empty,
                MotoIdentifier: string.Empty,
                DateStart: faker.Date.Past(),
                DateEnd: faker.Date.Past(),
                DatePreview: faker.Date.Past(),
                Plan: 0));
    }
}
