using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.TraineeDtos;

namespace SportAcademy.Application.Queries.TraineeQueries.GetCurrentTrainee
{
    public record GetCurrentTraineeQuery() : IRequest<Result<TraineeDetailsDto>>;
}
