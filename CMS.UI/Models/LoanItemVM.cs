using CMS.DAL.Entities;

namespace CMS.UI.Models
{
    public class LoanItemVM
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public List<Loan> Loans { get; set; }
        public Loan Loan { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public List<Product> Products { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
