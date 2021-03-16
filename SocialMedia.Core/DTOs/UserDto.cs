using System;

namespace SocialMedia.Core.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Telephone { get; set; }
        public bool IsActive { get; set; }
    }
}
