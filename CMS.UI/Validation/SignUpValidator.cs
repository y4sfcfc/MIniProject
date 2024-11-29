using FluentValidation;
using CMS.UI.Models;

namespace CMS.UI.Validation
{
	public class SignUpValidator:AbstractValidator<SignUpVM>
	{
		public SignUpValidator() {
			RuleFor(s=>s.UserName).NotEmpty().WithName("User Name");
			RuleFor(s=>s.Password).NotEmpty();
			RuleFor(s=>s.ConfirmPassword).Equal(s=>s.Password).WithMessage("Enter same password").WithName("Confirm Password");
			RuleFor(s=>s.PhoneNumber).NotNull().Must(s => s != null &&s.StartsWith("+994")&&s.Substring(4).All(char.IsDigit)).WithMessage("Phone number must start with +994").WithName("Phone number");
		}
	}
}
