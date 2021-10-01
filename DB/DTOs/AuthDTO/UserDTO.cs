namespace idolapi.DB.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string[] Roles { get; set; }
        public string AccessToken { get; set; }
    }
}