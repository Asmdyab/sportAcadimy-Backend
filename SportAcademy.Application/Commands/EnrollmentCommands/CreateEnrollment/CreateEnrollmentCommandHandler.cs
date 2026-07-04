using AutoMapper;
using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;
using SportAcademy.Domain.Exceptions.SubscriptonExceptions;
using SportAcademy.Domain.Services;

namespace SportAcademy.Application.Commands.EnrollmentCommands.CreateEnrollment
{
    public class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, Result<int>>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ISubscriptionDetailsRepository _subRepository;
        private readonly IMapper _mapper;
        private readonly string _operationType = OperationType.Add.ToString();

        public CreateEnrollmentCommandHandler(
            IEnrollmentRepository enrollmentRepository,
            ISubscriptionDetailsRepository subscriptionDetailsRepository,
            IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _subRepository = subscriptionDetailsRepository;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var enrollment = _mapper.Map<Enrollment>(request)
                ?? throw new AutoMapperMappingException("Error occurred while mapping.");

            var subDetails = await _subRepository.GetSubscriptionDetailsWithSubTypeAsync(
                request.SubscriptionDetailsId, cancellationToken)
                ?? throw new SubscriptionDetailsNotFoundException(request.SubscriptionDetailsId.ToString());

            if (DateOnly.FromDateTime(request.EnrollmentDate) > subDetails.EndDate)
            {
                throw new InvalidOperationException("Enrollment date cannot be after the subscription end date.");
            }

            // Calculate sessionAllowed from subscription
            enrollment.SessionAllowed = SubscriptionDetailsService.CalculateAllowedSessions(subDetails);

            enrollment.SessionRemaining = enrollment.SessionAllowed;
            enrollment.IsActive = true;

            cancellationToken.ThrowIfCancellationRequested();

            await _enrollmentRepository.AddAsync(enrollment, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            return Result<int>.Success(enrollment.Id, _operationType);
        }
    }
}
