using Job.Domain.Commons;
using Job.Domain.Enums;

namespace Job.Domain.Entities.User;

public sealed class MotoboyEntity : UserEntity
{
    private MotoboyEntity() : base(string.Empty)
    {
    }

    public MotoboyEntity(
        string identifier,
        string password,
        string name,
        string cnpj,
        DateOnly dateBirth,
        string cnh,
        ECnhType type) : base(password)
    {
        Identifier = identifier;
        Name = name;
        Cnpj = CnpjValidation.FormatCnpj(cnpj);
        DateBirth = dateBirth;
        Cnh = CnhValidation.FormatCnh(cnh);
        Type = type;
    }

    public string Identifier { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Cnpj { get; private set; } = string.Empty;
    public DateOnly DateBirth { get; private set; }
    public string Cnh { get; private set; } = string.Empty;
    public ECnhType Type { get; private set; }
    public string? CnhImage { get; private set; }

    public void UpdateCnhImage(string image)
    {
        Update();
        CnhImage = image;
    }
}