using MediatR;
using Microsoft.EntityFrameworkCore;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.SubscriptionTypeDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Queries.SubscriptionTypeQueries.GetSubTypesDropdown;

public class GetSubTypesDropdownQueryHandler : IRequestHandler<GetSubTypesDropdownQuery, Result<List<SubTypeDropdownDto>>>
{
    private readonly string _operation = OperationType.GetAll.ToString();
    private readonly ISubscriptionTypeRepository _subscriptionTypeRepository;

    public GetSubTypesDropdownQueryHandler(ISubscriptionTypeRepository subscriptionTypeRepository)
    {
        _subscriptionTypeRepository = subscriptionTypeRepository;
    }

    public async Task<Result<List<SubTypeDropdownDto>>> Handle(GetSubTypesDropdownQuery request, CancellationToken cancellationToken)
    {
        var items = await _subscriptionTypeRepository.GetAllAsync(cancellationToken);
        var dropdown = items
            .Where(s => s.IsActive)
            .Select(s => new SubTypeDropdownDto
            {
                Id = s.Id,
                Name = s.Name.ToString()
            })
            .ToList();

        return Result<List<SubTypeDropdownDto>>.Success(dropdown, _operation);
    }
}
