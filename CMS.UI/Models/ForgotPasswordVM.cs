using System.ComponentModel.DataAnnotations;

namespace CMS.UI.Models
{
    public class ForgotPasswordVM
    {
        [Required]
        public string Email { get; set; }
    }
}
