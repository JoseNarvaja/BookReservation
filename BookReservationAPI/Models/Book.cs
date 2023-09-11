using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReservationAPI.Models
{
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
        [StringLength(14)]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [ValidateNever]
        public string ImageUrl { get; set; }
        [Required]
        public int IdCategory { get; set; }
        [Required]
        [ForeignKey("IdCategory")]
        [ValidateNever]
        public Category category { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public float Price { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
