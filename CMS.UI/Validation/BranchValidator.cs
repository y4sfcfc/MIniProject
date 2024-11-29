using FluentValidation;
using CMS.DAL.Entities;
using CMS.UI.Models;
using System.Net;

namespace CMS.UI.Validation
{
	public class BranchValidator:AbstractValidator<BranchVM>
	{
        public BranchValidator()
        {


			RuleFor(b=>b.Name).NotEmpty().MaximumLength(50);
			RuleFor(b=>b.Description).NotEmpty().MaximumLength(100);
			RuleFor(b=>b.Address).NotEmpty().MaximumLength(30);
			RuleFor(b=>b.AppUserId).NotNull();
        }
    }
}
