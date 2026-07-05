using AutoMapper;
using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.SubscriptionDetailsDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Queries.SubscriptionDetailsQueries.GetByTrainee
{
    public class GetSubDetailsByTraineeQueryHandler : IRequestHandler<GetSubDetailsByTraineeQuery, Result<List<SubscriptionDetailsDto>>>
    {
        private readonly string _operation = OperationType.GetAll.ToString();
        private readonly ISubscriptionDetailsRepository _subscriptionDetailsRepository;
        private readonly IMapper _mapper;

        public GetSubDetailsByTraineeQueryHandler(
            ISubscriptionDetailsRepository subscriptionDetailsRepository,
            IMapper mapper)
        {
            _subscriptionDetailsRepository = subscriptionDetailsRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<SubscriptionDetailsDto>>> Handle(GetSubDetailsByTraineeQuery request, CancellationToken cancellationToken)
        {
            var subDetailsList = await _subscriptionDetailsRepository.GetSubscriptionDetailsForTraineeAsync(request.TraineeId, cancellationToken)
                ?? [];

            cancellationToken.ThrowIfCancellationRequested();

            var subDetailsDtoList = _mapper.Map<List<SubscriptionDetailsDto>>(subDetailsList)
                ?? [];

            return Result<List<SubscriptionDetailsDto>>.Success(subDetailsDtoList, _operation);
        }
    }
}
