using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.TraineeGroupDtos;

namespace SportAcademy.Application.Queries.TraineeGroupQueries.GetDropdown;

public record GetTraineeGroupsDropdownQuery() : IRequest<Result<List<TraineeGroupDropdownDto>>>;
