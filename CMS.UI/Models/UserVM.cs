using CMS.DAL.Entities;

namespace CMS.UI.Models
{
    public class UserVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        public IList<AppRole> RoleList { get; set; }
        public string PhoneNumber { get; set; }
        public string? ConnectionId { get; set; }

    }
}
