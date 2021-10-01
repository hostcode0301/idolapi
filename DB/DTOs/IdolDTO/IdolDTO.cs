namespace idolapi.DB.DTOs
{
    public class IdolDTO
    {
        public int IdolId { get; set; }
        public string IdolNameJP { get; set; }
        public string IdolNameRomaji { get; set; }
        public string IdolNameHira { get; set; }
        public string DOB { get; set; }
        public float Bust { get; set; }
        public float Waist { get; set; }
        public float Hip { get; set; }
        public float Height { get; set; }
        public string ImageURL { get; set; }
    }
}