namespace Job.Domain.Entities.Moto;

public sealed class MotoEntity : BaseEntity
{
    private MotoEntity()
    {
    }

    public MotoEntity(string identifier, int year, string model, string plate)
    {
        Identifier = identifier;
        Year = year;
        Model = model;
        Plate = plate.Replace("-", string.Empty);
    }

    public string Identifier { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public string Plate { get; private set; } = string.Empty;

    public void UpdatePlate(string plate)
    {
        Update();
        Plate = plate.Replace("-", string.Empty);
    }
}