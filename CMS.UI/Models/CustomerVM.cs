using CMS.DAL.Entities;

namespace CMS.UI.Models
{
    public class CustomerVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Occupation { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public List<AppUser> Users { get; set; }
        public List<Loan> Loans { get; set; }
        public List<LoanItemVM> LoanItems { get; set; }
        public LoanDetail LoanDetail { get; set; }
    }
}
