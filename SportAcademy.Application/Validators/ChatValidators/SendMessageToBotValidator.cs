using FluentValidation;
using SportAcademy.Application.Commands.ChatCommands.SendMessageToBot;

namespace SportAcademy.Application.Validators.ChatValidators
{
    public class SendMessageToBotValidator
    : AbstractValidator<SendMessageToBotCommand>
    {
        public SendMessageToBotValidator()
        {
            RuleFor(x => x.ConversationId)
                .NotEmpty();

            RuleFor(x => x.Message)
                .NotEmpty()
                .MaximumLength(2000);
        }
    }
}
