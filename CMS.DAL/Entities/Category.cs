using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Entities
{
    public class Category:BaseEntity
    {
        public string Name { get; set; }
        public int ParentId { get; set; }
        public int Level { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
