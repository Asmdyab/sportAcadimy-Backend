using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.SubscriptionDetailsDtos;

namespace SportAcademy.Application.Queries.SubscriptionDetailsQueries.GetByTrainee
{
    public record GetSubDetailsByTraineeQuery(int TraineeId) : IRequest<Result<List<SubscriptionDetailsDto>>>;
}
