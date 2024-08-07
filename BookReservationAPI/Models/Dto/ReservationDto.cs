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
        public string BookTitle { get; set; }
        [Required]
        public string UserUsername { get; set; }
        [Required]
        public string CopyBarcode { get; set; }
    }
}
