using System.ComponentModel.DataAnnotations;

namespace idolapi.DB.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string[] Roles { get; set; }
    }
}