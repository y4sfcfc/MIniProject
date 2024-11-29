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
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
           builder.HasOne(b=>b.Merchant).WithMany(m=>m.Branchs).HasForeignKey(b=>b.MerchantId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
