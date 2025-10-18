using ChatbotAI_BE.Dtos.AI;
using FluentValidation;

namespace ChatbotAI_BE.Validators.AI
{
    public class AskAIRequestValidator : AbstractValidator<AskAIRequest>
    {
        public AskAIRequestValidator()
        {
            RuleFor(x => x.ModelId)
                .NotEmpty().WithMessage("ModelId không được để trống.");

            RuleFor(x => x.UserMessage)
                .NotEmpty().WithMessage("Tin nhắn không được để trống.")
                .MaximumLength(5000).WithMessage("Tin nhắn không được vượt quá 5000 ký tự.")
                .MinimumLength(1).WithMessage("Tin nhắn phải có ít nhất 1 ký tự.");

            RuleFor(x => x.SessionId)
                .Must(x => x == null || x != Guid.Empty)
                .WithMessage("SessionId không hợp lệ.");
        }
    }
}
