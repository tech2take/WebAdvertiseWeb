
using System.ComponentModel.DataAnnotations;

namespace WebAdvertiseWeb.Models.Accounts
{
    public class SignupModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(6,ErrorMessage ="Password must be six character long")]
        [Display(Name = "Password")]
        public string  Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password" ,   ErrorMessage ="Password do not match")]
        [Display(Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}
