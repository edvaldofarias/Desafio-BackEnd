using Job.Domain.Commands;
using MediatR;

namespace Job.Domain.UseCases.Moto.Delete.Commands;

public sealed record DeleteMotoCommand(Guid Id) : IRequest<CommandResponse<string>>;