using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReservationAPI.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime ReservationDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime ReservationEnd { get; set; }
        [DataType(DataType.Date)]
        public DateTime? PickupDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        [Required]
        public int BookId { get; set; }
        [Required]
        [ValidateNever]
        [ForeignKey("BookId")]
        public Book Book { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [ValidateNever]
        [ForeignKey("UserId")]
        public LocalUser User { get; set; }
        [Required]
        public int CopyId { get; set; }
        [Required]
        [ValidateNever]
        [ForeignKey("CopyId")]
        public Copy Copy { get; set; }
    }
}
