using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Entities
{
	public class ProductImage:BaseEntity
	{
        public string Name { get; set; }
        public string Directory { get; set; }
    }
}
