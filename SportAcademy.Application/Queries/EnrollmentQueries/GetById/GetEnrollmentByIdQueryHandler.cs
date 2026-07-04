using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.EnrollmentDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;
using SportAcademy.Domain.Exceptions.EnrollmentExceptions;

namespace SportAcademy.Application.Queries.EnrollmentQueries.GetById
{
    public class GetEnrollmentByIdQueryHandler : IRequestHandler<GetEnrollmentByIdQuery, Result<EnrollmentCardDto>>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IMapper _mapper;
        private readonly string _operationType = OperationType.Get.ToString();

        public GetEnrollmentByIdQueryHandler(
            IEnrollmentRepository enrollmentRepository,
            IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
        }

        public async Task<Result<EnrollmentCardDto>> Handle(GetEnrollmentByIdQuery request, CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new EnrollmentNotFoundException($"{request.Id}");

            var enrollmentDto = _mapper.Map<EnrollmentCardDto>(enrollment)
                ?? throw new AutoMapperMappingException("Error occurred while mapping.");

            return Result<EnrollmentCardDto>.Success(enrollmentDto, _operationType);
        }
    }
}
