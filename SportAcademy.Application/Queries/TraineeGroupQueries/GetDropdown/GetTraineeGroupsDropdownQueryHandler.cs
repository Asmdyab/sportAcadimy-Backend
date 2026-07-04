using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.TraineeGroupDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Queries.TraineeGroupQueries.GetDropdown;

public class GetTraineeGroupsDropdownQueryHandler : IRequestHandler<GetTraineeGroupsDropdownQuery, Result<List<TraineeGroupDropdownDto>>>
{
    private readonly ITraineeGroupRepository _traineeGroupRepository;
    private readonly string _operationType = OperationType.Get.ToString();

    public GetTraineeGroupsDropdownQueryHandler(ITraineeGroupRepository traineeGroupRepository)
    {
        _traineeGroupRepository = traineeGroupRepository;
    }

    public async Task<Result<List<TraineeGroupDropdownDto>>> Handle(GetTraineeGroupsDropdownQuery request, CancellationToken cancellationToken)
    {
        var groups = await _traineeGroupRepository.GetDropdownListAsync(cancellationToken);
        return Result<List<TraineeGroupDropdownDto>>.Success(groups, _operationType);
    }
}
