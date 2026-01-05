using System.ComponentModel.DataAnnotations;

namespace MVC_App.ViewModels.Account
{
    public class RegisterVM
    {
        [Required,MaxLength(32),MinLength(2)]
        public string FullName { get; set; }=string.Empty;
        [Required, MaxLength(256), EmailAddress]

        public string Email { get; set; }= string.Empty;
        [Required, MaxLength(256), MinLength(5),DataType(DataType.Password)]

        public string Password { get; set; }=string.Empty ;
        [Required, MaxLength(256), MinLength(5), DataType(DataType.Password), Compare(nameof(Password))]

        public string ConfirmPassword { get; set; } = string.Empty;
        public string UserName { get; set; }

    }
}
