using CMS.DAL.Entities;

namespace CMS.UI.Models
{
    public class BranchVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int MerchantId { get; set; }
        public string MerchantName { get; set; }
        public List<Merchant> Merchants { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
