using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookReservationAPI.Models.Dto
{
    public class ReservationDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime ReservationDate { get; set; }
        [Required]
        public DateTime ReservationEnd { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        [Required]
        public int BookId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int CopyId { get; set; }
    }
}
