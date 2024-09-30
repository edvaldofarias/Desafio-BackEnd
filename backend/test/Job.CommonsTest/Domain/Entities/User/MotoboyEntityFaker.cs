using Bogus;
using Bogus.Extensions.Brazil;
using Job.Domain.Entities.User;
using Job.Domain.Enums;

namespace Job.Commons.Domain.Entities.User;

public static class MotoboyEntityFaker
{
    public static Faker<MotoboyEntity> Default(string? password = null)
    {
        return new Faker<MotoboyEntity>()
            .CustomInstantiator(faker => new MotoboyEntity(
                BCrypt.Net.BCrypt.HashPassword(password, 12),
                faker.Person.FullName,
                faker.Company.Cnpj(),
                DateOnly.FromDateTime(faker.Person.DateOfBirth),
                "77058710884",
                faker.PickRandom<ECnhType>()));
    }

    public static Faker<MotoboyEntity> DefaultWithTypeCnh(ECnhType type)
    {
        return new Faker<MotoboyEntity>()
            .CustomInstantiator(faker => new MotoboyEntity(
                faker.Internet.Password(),
                faker.Person.FullName,
                faker.Company.Cnpj(),
                DateOnly.FromDateTime(faker.Person.DateOfBirth),
                "77058710884",
                type));
    }
}