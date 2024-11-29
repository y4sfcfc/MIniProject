using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CMS.DAL.Entities;

namespace CMS.DAL.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {

            builder.HasOne(x => x.Branch)
          .WithMany(x => x.Categories) // No collection on the Branch side
          .HasForeignKey(x => x.BranchId)
          .OnDelete(DeleteBehavior.Restrict);

            // A Category is added by one Employee
            builder.HasOne(x => x.Employee)
                .WithMany(x => x.Categories) // No collection on the Employee side
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
