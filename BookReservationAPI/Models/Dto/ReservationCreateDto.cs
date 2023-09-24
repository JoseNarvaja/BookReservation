using System.ComponentModel.DataAnnotations;

namespace BookReservationAPI.Models.Dto
{
    public class ReservationCreateDto
    {
        [Required]
        public DateTime ReservationDate { get; set; }
        [Required]
        public DateTime ReservationEnd { get; set; }

        [Required]
        [StringLength(14)]
        public string ISBN { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
