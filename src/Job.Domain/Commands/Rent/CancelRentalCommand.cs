namespace Job.Domain.Commands.Rent;

public sealed record CancelRentalCommand(Guid Id, DateTime DatePreview);