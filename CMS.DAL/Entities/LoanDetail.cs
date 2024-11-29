using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Entities
{
    public class LoanDetail:BaseEntity
    {
        public int LoanId { get; set; }
        public Loan Loan { get; set; }
        public decimal CurrentAmount { get; set; }
    }
}
