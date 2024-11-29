using System.ComponentModel.DataAnnotations;

namespace CMS.UI.Models
{
    public class LogInVM
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool Persistent { get; set; }
    }
}
