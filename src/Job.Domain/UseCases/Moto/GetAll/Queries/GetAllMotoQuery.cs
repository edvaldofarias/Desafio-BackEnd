using Job.Domain.UseCases.Moto.GetAll.Responses;
using MediatR;

namespace Job.Domain.UseCases.Moto.GetAll.Queries;

public record GetAllMotoQuery(int Quantity = 10, int Page = 0) : IRequest<IEnumerable<MotoResponse>>;
