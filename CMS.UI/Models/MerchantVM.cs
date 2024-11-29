    using CMS.DAL.Entities;

namespace CMS.UI.Models
{
    public class MerchantVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TerminalNo { get; set; }
        public string AppUserId { get; set; }
    }
}
