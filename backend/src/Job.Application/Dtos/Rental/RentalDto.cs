using Job.Domain.Enums;

namespace Job.Application.Dtos.Rental;

public record RentalDto(Guid Id, decimal Value, EPlan Plan, decimal? Fine = null);