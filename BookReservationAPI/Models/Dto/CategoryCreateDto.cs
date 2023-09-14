using System.ComponentModel.DataAnnotations;

namespace BookReservationAPI.Models.Dto
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(40)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
