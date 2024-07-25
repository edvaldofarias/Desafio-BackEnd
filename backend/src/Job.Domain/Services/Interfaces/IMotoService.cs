﻿using Job.Domain.Commands;
using Job.Domain.Commands.Moto;
using Job.Domain.Dtos.Moto;

namespace Job.Domain.Services.Interfaces;

public interface IMotoService
{
    Task<CommandResponse<string>> CreateAsync(CreateMotoCommand command, CancellationToken cancellationToken);
    Task<CommandResponse<string>> UpdateAsync(UpdateMotoCommand command, CancellationToken cancellationToken);
    Task<CommandResponse<string>> DeleteAsync(Guid idMoto, CancellationToken cancellationToken);
    Task<IEnumerable<MotoDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<MotoDto?> GetByIdAsync(Guid idMoto, CancellationToken cancellationToken);
    Task<MotoDto?> GetByPlateAsync(string placa, CancellationToken cancellationToken);
}