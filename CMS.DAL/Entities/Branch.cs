using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Entities
{
    public class Branch:BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int MerchantId { get; set; }
        public Merchant Merchant { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<Category> Categories { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Product> Products { get; set; }
    }
}
