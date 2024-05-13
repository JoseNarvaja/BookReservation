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
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
