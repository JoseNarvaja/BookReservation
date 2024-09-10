using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookReservationAPI.Models.Dto
{
    public class BookDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [StringLength(13)]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [ValidateNever]
        public string ImageUrl { get; set; }
        [Required]
        public int IdCategory { get; set; }
        public Category category { get; set; }
    }
}
