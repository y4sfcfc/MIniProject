using CMS.DAL.Entities;

namespace CMS.UI.Models
{
	public class ProductVM
	{
        public int Id { get; set; }
		public string Name { get; set; }
		public int CategoryId { get; set; }
		public string CategoryName { get; set; }
        public List<Category> Categories { get; set; }
		public int BranchId { get; set; }
		public string BranchName { get; set; }
        public List<Branch> Branchs { get; set; }
        public decimal Price { get; set; }
		public string Description { get; set; }
		public int Count { get; set; }
		public string Model { get; set; }
		public string Brand { get; set; }
        public IFormFile[] ImageFiles { get; set; }
        public string Thumbnail { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<ProductVM> RelatedProducts { get; set; }

    }
}
