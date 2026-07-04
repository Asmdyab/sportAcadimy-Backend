using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Commands.PaymentCommands.CreatePayment
{
    public record CreatePaymentCommand(
        PaymentMethod Method,
        int BranchId,
        DateTime? PaidDate
    ) : IRequest<Result<string>>;
}
