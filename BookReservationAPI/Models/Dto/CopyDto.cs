using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookReservationAPI.Models.Dto
{
    public class CopyDto
    {
        public string Barcode { get; set; }
        [Required]
        public int BookId { get; set; }
    }
}
