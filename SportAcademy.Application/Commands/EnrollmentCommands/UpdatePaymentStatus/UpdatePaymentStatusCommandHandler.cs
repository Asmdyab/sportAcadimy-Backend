using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.Interfaces;

namespace SportAcademy.Application.Commands.EnrollmentCommands.UpdatePaymentStatus;

public class UpdatePaymentStatusCommandHandler : IRequestHandler<UpdatePaymentStatusCommand, Result<bool>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly string _operationType = "UpdatePaymentStatus";

    public UpdatePaymentStatusCommandHandler(
        IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<Result<bool>> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (enrollment == null)
            return Result<bool>.Failure(_operationType, "Enrollment not found", 404);

        switch (request.PaymentStatus.ToLowerInvariant())
        {
            case "paid":
                enrollment.SessionRemaining = 0;
                break;
            case "pending":
                enrollment.SessionRemaining = enrollment.SessionAllowed;
                break;
        }

        await _enrollmentRepository.UpdateAsync(enrollment, cancellationToken);

        return Result<bool>.Success(true, _operationType);
    }
}
