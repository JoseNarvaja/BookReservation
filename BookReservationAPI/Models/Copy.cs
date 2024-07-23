using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReservationAPI.Models
{
    [Index(nameof(Barcode), IsUnique = true)]
    public class Copy
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "The Barcode must be numeric and 13 digits long.")]
        public string Barcode { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        [Required]
        public int BookId { get; set; }
        [ForeignKey("BookId")]
        [ValidateNever]
        public Book Book { get; set; }
        [Required]
        public bool IsDeleted { get; set; } = false;
    }
}
