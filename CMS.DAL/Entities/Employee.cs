using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Entities
{
    public class Employee:BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Position { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<Category> Categories { get; set; }
    }
}
