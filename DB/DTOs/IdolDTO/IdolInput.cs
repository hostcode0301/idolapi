using System.ComponentModel.DataAnnotations;

namespace idolapi.DB.DTOs
{
    public class IdolInput
    {
        [Required]
        public string IdolNameJP { get; set; }
        [Required]
        public string IdolNameRomaji { get; set; }
        [Required]
        public string IdolNameHira { get; set; }
        [Required]
        public string DOB { get; set; }
        [Required]
        public float Bust { get; set; }
        [Required]
        public float Waist { get; set; }
        [Required]
        public float Hip { get; set; }
        [Required]
        public float Height { get; set; }
        [Required]
        public string ImageURL { get; set; }
    }
}