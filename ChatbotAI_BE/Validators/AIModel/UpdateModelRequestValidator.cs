using ChatbotAI_BE.Dtos.AIModel;
using FluentValidation;

namespace ChatbotAI_BE.Validators.AIModel
{
    public class UpdateModelRequestValidator : AbstractValidator<UpdateModelRequest>
    {
        public UpdateModelRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code không được để trống.")
                .MaximumLength(100).WithMessage("Code tối đa 100 ký tự.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên không được để trống.")
                .MaximumLength(200).WithMessage("Tên tối đa 200 ký tự.");

            RuleFor(x => x.MaxTokens)
                .GreaterThan(0).WithMessage("MaxTokens phải lớn hơn 0.");

            RuleFor(x => x.TotalContext)
                .GreaterThan(0).WithMessage("TotalContext phải lớn hơn 0.");
        }
    }
}
