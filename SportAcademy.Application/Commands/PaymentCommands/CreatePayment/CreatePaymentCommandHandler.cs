using AutoMapper;
using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.Interfaces;
using SportAcademy.Application.Services.PaymentServices;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Commands.PaymentCommands.CreatePayment
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<string>>
    {
        private readonly string _operation = OperationType.Add.ToString();
        private readonly IPaymentRepository _paymentRepository;
        private readonly PaymentNumberService _paymentNumberService;
        private readonly IMapper _mapper;

        public CreatePaymentCommandHandler(
            IPaymentRepository paymentRepository,
            PaymentNumberService paymentNumberService,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _paymentNumberService = paymentNumberService;
            _mapper = mapper;
        }

        public async Task<Result<string>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var paymentNumber = await _paymentNumberService.GenerateNextPaymentNumberAsync(cancellationToken);

            var payment = new Payment
            {
                PaymentNumber = paymentNumber,
                Method = request.Method,
                PaidDate = request.PaidDate ?? DateTime.Now,
                BranchId = request.BranchId
            };

            await _paymentRepository.AddAsync(payment, cancellationToken);

            return Result<string>.Success(paymentNumber, _operation);
        }
    }
}
