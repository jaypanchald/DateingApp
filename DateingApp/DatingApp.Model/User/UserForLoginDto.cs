using System.ComponentModel.DataAnnotations;

namespace DatingApp.Model.User
{
    public class UserForLoginDto
    {
        [Required, EmailAddress]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
