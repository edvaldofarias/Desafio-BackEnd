using Job.Domain.Commands;
using MediatR;

namespace Job.Domain.UseCases.Moto.Update.Commands;

public sealed record UpdateMotoCommand(Guid Id, int Year, string Model, string Plate) : IRequest<CommandResponse<string>>;