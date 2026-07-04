using AutoMapper;
using SportAcademy.Application.Commands.PaymentCommands.CreatePayment;
using SportAcademy.Domain.Entities;

namespace SportAcademy.Application.Mappings.PaymentProfile
{
    public class PaymentProfile : AutoMapper.Profile
    {
        public PaymentProfile()
        {
            CreateMap<CreatePaymentCommand, Payment>();
        }
    }
}
