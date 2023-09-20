using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookReservationAPI.Models
{
    public class LocalUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
    }
}
