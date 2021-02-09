using Dating.Model.Photo;
using System;
using System.Collections.Generic;

namespace Dating.Model.User
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
    }

    public class UserDataDto : UserListDto
    {
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }

        public List<PhotoDto> Photos { get; set; }
    }

    public class UserForUpdateDto
    {
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
