using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReservationAPI.Models
{
    [Index(nameof(ISBN), IsUnique =true)]
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [StringLength(13)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The ISBN can only contain numbers.")]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }
        [ValidateNever]
        public string? ImageId { get; set; }
        [Required]
        public int IdCategory { get; set; }
        [ForeignKey("IdCategory")]
        [ValidateNever]
        public Category category { get; set; }
        public ICollection<Copy> Copies { get; set; }  = new List<Copy>();
    }
}
