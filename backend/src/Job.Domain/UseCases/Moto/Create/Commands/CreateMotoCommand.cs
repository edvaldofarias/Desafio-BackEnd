using Job.Domain.Commands;
using MediatR;

namespace Job.Domain.UseCases.Moto.Create.Commands;
public sealed record CreateMotoCommand(int Year, string Model, string Plate) : IRequest<CommandResponse<string>>;