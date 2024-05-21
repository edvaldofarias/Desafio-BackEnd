namespace Job.Domain.UseCases.Moto.GetAll.Responses;

public sealed record MotoResponse(Guid Id, int Year, string Model, string Plate);
