using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReservationAPI.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime ReservationDate { get; set; }
        [Required]
        public DateTime ReservationEnd { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }

        [Required]
        public int BookId { get; set; }
        [Required]
        [ForeignKey("BookId")]
        public Book Book { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public LocalUser User { get; set; }
    }
}
