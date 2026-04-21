namespace Job.Application.Messaging;

public sealed record MotoCreatedEvent(Guid MotoId, int Year, string Model, string Plate, DateTime OccurredAt);
