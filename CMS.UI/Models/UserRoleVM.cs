namespace CMS.UI.Models
{
    public class UserRoleVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RoleVM> Roles { get; set; }
    }
}
