using Microsoft.EntityFrameworkCore;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;

namespace SportAcademy.Application.Services.PaymentServices
{
    public class PaymentNumberService
    {
        private readonly IBaseRepository<Payment, string> _paymentRepository;

        public PaymentNumberService(IBaseRepository<Payment, string> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<string> GenerateNextPaymentNumberAsync(CancellationToken cancellationToken = default)
        {
            var year = DateTime.UtcNow.Year;
            var allPayments = await _paymentRepository.GetAllAsync(cancellationToken);
            var maxNumber = allPayments
                ?.Where(p => p.PaymentNumber.StartsWith($"PAY-{year}-"))
                .Select(p => p.PaymentNumber)
                .DefaultIfEmpty($"PAY-{year}-{0:D5}")
                .Max();

            var parts = maxNumber?.Split('-');
            if (parts?.Length == 3 && int.TryParse(parts[2], out var lastCounter))
                return $"PAY-{year}-{lastCounter + 1:D5}";

            return $"PAY-{year}-{10000:D5}";
        }
    }
}
