using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CMS.DAL.Configuration;
using CMS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Data
{
    public class CMSDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public CMSDbContext(DbContextOptions<CMSDbContext> opts) : base(opts)
        {

        }
	
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Branch> Branchs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanDetail> LoanDetails { get; set; }
        public DbSet<LoanItem> LoanItems { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Product> Products { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<Slider> Sliders { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(CategoryConfiguration).Assembly);
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(BranchConfiguration).Assembly);
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentConfiguration).Assembly);
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoanItemConfiguration).Assembly);
			base.OnModelCreating(modelBuilder);
		}

	}
}
