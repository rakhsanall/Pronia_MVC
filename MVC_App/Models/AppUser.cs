using Microsoft.AspNetCore.Identity;

namespace MVC_App.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
