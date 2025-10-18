using ChatbotAI_BE.Dtos.Activity;
using FluentValidation;

namespace ChatbotAI_BE.Validators.Activity
{
    public class AddActivityRequestValidator : AbstractValidator<AddActivityRequest>
    {
        public AddActivityRequestValidator()
        {
            RuleFor(x => x.ModelId)
                .NotEmpty().WithMessage("ModelId không được để trống.");

            RuleFor(x => x.InputTokens)
                .GreaterThanOrEqualTo(0).WithMessage("InputTokens phải >= 0.");

            RuleFor(x => x.OutputTokens)
                .GreaterThanOrEqualTo(0).WithMessage("OutputTokens phải >= 0.");
        }
    }
}
