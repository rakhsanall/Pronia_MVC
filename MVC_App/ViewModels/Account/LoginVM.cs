using System.ComponentModel.DataAnnotations;

namespace MVC_App.ViewModels.Account
{
    public class LoginVM
    {
        [Required, MaxLength(100), EmailAddress]
        public string Email { get; set; }=string.Empty;

        [Required, MaxLength(50),MinLength(6), DataType(DataType.Password)]

        public string Password { get; set; } = string.Empty;
        public bool IsRemember { get; set; }
       
        
    }
}
