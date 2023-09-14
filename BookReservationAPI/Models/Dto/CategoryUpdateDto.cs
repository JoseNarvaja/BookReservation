using System.ComponentModel.DataAnnotations;

namespace BookReservationAPI.Models.Dto
{
    public class CategoryUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
