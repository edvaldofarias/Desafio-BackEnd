using Bogus;
using Job.Domain.Entities.Moto;

namespace Job.Commons.Domain.Entities.Moto;

public static class MotoEntityFaker
{
    public static Faker<MotoEntity> Default()
    {
        return new Faker<MotoEntity>()
            .CustomInstantiator(faker => new MotoEntity(
                faker.Random.AlphaNumeric(8),
                faker.Random.Int(1901, 2050),
                $"{faker.Vehicle.Model()}-{faker.Random.AlphaNumeric(3)}",
                faker.Vehicle.Vin()
            ));
    }
}
