using ChatbotAI_BE.Dtos.Session;
using FluentValidation;

namespace ChatbotAI_BE.Validators.Session
{
    public class CreateSessionRequestValidator : AbstractValidator<CreateSessionRequest>
    {
        public CreateSessionRequestValidator()
        {
            RuleFor(x => x.ModelId)
                .NotEmpty().WithMessage("ModelId là bắt buộc.");

            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Tiêu đề không được vượt quá 200 ký tự.");
        }
    }
}
