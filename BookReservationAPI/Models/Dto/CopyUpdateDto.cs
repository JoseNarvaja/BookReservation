using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookReservationAPI.Models.Dto
{
    public class CopyUpdateDto
    {
        public string Barcode { get; set; }
        public string ISBN { get; set; }
    }
}
