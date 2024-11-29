using FluentValidation;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using CMS.DAL.Entities;
using CMS.UI.Models;

namespace CMS.UI.Validation
{
	public class EmployeeValidator:AbstractValidator<EmployeeVM>
	{
        public EmployeeValidator()
        {
			RuleFor(m => m.Name).MaximumLength(50).NotEmpty();
			RuleFor(m => m.Surname).MaximumLength(50).NotEmpty();
			RuleFor(m => m.Position).MaximumLength(30).NotEmpty();
			RuleFor(m => m.BranchId).NotNull().WithMessage("Branch required");
			RuleFor(m => m.AppUserId).NotNull();
		}
    }
}
