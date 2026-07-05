using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.TraineeDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;
using SportAcademy.Domain.Exceptions.TraineeExceptions;

namespace SportAcademy.Application.Queries.TraineeQueries.GetCurrentTrainee
{
    public class GetCurrentTraineeQueryHandler : IRequestHandler<GetCurrentTraineeQuery, Result<TraineeDetailsDto>>
    {
        private readonly ITraineeRepository _traineeRepository;
        private readonly IUserContextService _userContext;
        private readonly string _operationType = OperationType.Get.ToString();

        public GetCurrentTraineeQueryHandler(
            ITraineeRepository traineeRepository,
            IUserContextService userContext)
        {
            _traineeRepository = traineeRepository;
            _userContext = userContext;
        }

        public async Task<Result<TraineeDetailsDto>> Handle(GetCurrentTraineeQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContext.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated.");

            var trainee = await _traineeRepository.GetByAppUserIdAsync(userId, cancellationToken)
                ?? throw new TraineeNotFoundException($"Trainee not found for user {userId}");

            return Result<TraineeDetailsDto>.Success(trainee, _operationType);
        }
    }
}
