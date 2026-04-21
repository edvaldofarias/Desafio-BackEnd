namespace Job.Domain.Entities.Notification;

public sealed class MotoNotificationEntity : BaseEntity
{
    private MotoNotificationEntity()
    {
    }

    public MotoNotificationEntity(Guid motoId, int year, string model, string plate, DateTime occurredAt)
    {
        MotoId = motoId;
        Year = year;
        Model = model;
        Plate = plate;
        OccurredAt = occurredAt;
    }

    public Guid MotoId { get; private set; }
    public int Year { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public string Plate { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }
}
