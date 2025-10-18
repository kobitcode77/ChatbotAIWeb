using ChatbotAI_BE.Dtos.Auth;
using FluentValidation;

namespace ChatbotAI_BE.Validators.Auth
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username không được để trống.")
                .MaximumLength(50).WithMessage("Username tối đa 50 ký tự.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password không được để trống.")
                .MinimumLength(6).WithMessage("Password phải ít nhất 6 ký tự.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email không hợp lệ.")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }
    }
}
