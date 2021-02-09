using Microsoft.AspNetCore.Identity;

namespace Dating.Model.Entity
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public User User { get; set; }

        public AppRole Role { get; set; }
    }
}
