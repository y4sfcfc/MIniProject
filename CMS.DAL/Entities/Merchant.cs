using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Entities
{
    public class Merchant:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TerminalNo { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<Branch> Branchs { get; set; }
    }
}
