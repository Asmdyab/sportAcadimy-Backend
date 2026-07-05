using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.EnrollmentDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Queries.EnrollmentQueries.GetByTrainee
{
    public class GetEnrollmentsByTraineeQueryHandler : IRequestHandler<GetEnrollmentsByTraineeQuery, Result<PagedData<EnrollmentCardDto>>>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly string _operationType = OperationType.GetAll.ToString();

        public GetEnrollmentsByTraineeQueryHandler(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<Result<PagedData<EnrollmentCardDto>>> Handle(GetEnrollmentsByTraineeQuery request, CancellationToken cancellationToken)
        {
            var result = await _enrollmentRepository.GetByTraineeIdAsync(
                request.TraineeId, request.Page, cancellationToken);

            return Result<PagedData<EnrollmentCardDto>>.Success(result, _operationType);
        }
    }
}
