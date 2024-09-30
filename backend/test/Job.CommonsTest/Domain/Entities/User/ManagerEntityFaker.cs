using Bogus;
using Job.Domain.Entities.User;

namespace Job.Commons.Domain.Entities.User;

public static class ManagerEntityFaker
{
    public static Faker<ManagerEntity> Default(string password)
    {
        return new Faker<ManagerEntity>()
            .CustomInstantiator(faker => new ManagerEntity(
                faker.Internet.Email(),
                BCrypt.Net.BCrypt.HashPassword(password, 12)
            ));
    }
}