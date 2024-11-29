using CMS.DAL.Entities;

namespace CMS.UI.Models
{
	public class LoanDetailVM
	{
        public int Id { get; set; }
        public int LoanId { get; set; }
        public Loan Loan { get; set; }
        public decimal CurrentAmount { get; set; }
    }
}
