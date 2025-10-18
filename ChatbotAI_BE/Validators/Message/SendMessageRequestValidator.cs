using ChatbotAI_BE.Dtos;
using FluentValidation;

namespace ChatbotAI_BE.Validators.Message
{
    public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
    {
        public SendMessageRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Nội dung tin nhắn không được để trống.")
                .MaximumLength(5000).WithMessage("Tin nhắn không được vượt quá 5000 ký tự.")
             .MinimumLength(1).WithMessage("Tin nhắn phải có ít nhất 1 ký tự.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Vai trò không hợp lệ.");
        }
    }
}
