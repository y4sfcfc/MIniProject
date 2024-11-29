using FluentValidation;
using CMS.UI.Models;

namespace CMS.UI.Validation
{
	public class MerchantValidator:AbstractValidator<MerchantVM>
	{
        public MerchantValidator()
        {
            RuleFor(m => m.Name).MaximumLength(50).NotEmpty();
            RuleFor(m => m.Description).MaximumLength(100).NotEmpty();
            RuleFor(m => m.TerminalNo).Length(16);
            RuleFor(m => m.AppUserId).NotNull();
        }
    }
}
