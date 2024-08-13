using Job.Domain.Enums;

namespace Job.Domain.Dtos.Rental;

public record RentalDto(Guid Id, decimal Value, EPlan Plan, decimal? Fine = null);