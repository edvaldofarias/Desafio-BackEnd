using Bogus;
using Job.Domain.Entities.Rental;
using Job.Domain.Enums;

namespace Job.Commons.Domain.Entities.Rental;

public static class RentalEntityFaker
{
    public static Faker<RentalEntity> Default()
    {
        return new Faker<RentalEntity>()
            .CustomInstantiator(faker =>
            {
                var start = faker.Date.FutureDateOnly();
                var preview = start.AddDays(7);
                var end = preview.AddDays(1);
                return new RentalEntity(
                    faker.Random.AlphaNumeric(8),
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    start,
                    end,
                    preview,
                    EPlan.Sete);
            });
    }
}
