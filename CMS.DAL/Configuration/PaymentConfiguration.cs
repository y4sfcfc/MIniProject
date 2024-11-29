using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CMS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Configuration
{
	public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
	{
		public void Configure(EntityTypeBuilder<Payment> builder)
		{
			// A Category is added by one Employee
			builder.HasOne(x => x.Loan)
				.WithMany(x => x.Payments) // No collection on the Employee side
				.HasForeignKey(x => x.LoanId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Customer)
	   .WithMany(x => x.Payments) // No collection on the Branch side
	   .HasForeignKey(x => x.CustomerId)
	   .OnDelete(DeleteBehavior.Restrict);

		}
	}
}
