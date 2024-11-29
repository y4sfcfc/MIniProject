using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Entities
{
    public class Payment:BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public int LoanId { get; set; }
        public Loan Loan { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
