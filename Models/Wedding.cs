using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeddingPlanner.Validators;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }

        [Required(ErrorMessage = "Must enter a Wedding Date")]
        [Display(Name = "Wedding Date")]
        [FutureDate]
        public DateTime WeddingDate { get; set; }

        [Required(ErrorMessage = "Must have a Wedder")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
        public string Wedder { get; set; }

        [Required(ErrorMessage = "Must have a Victim of the Wedder")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
        public string Victim { get; set; }

       [Required(ErrorMessage = "Must enter an Address")]
        [Display(Name = "Venue Address")]
        [MinLength(2, ErrorMessage = "Address must be at least 3 characters")]
        public string Address { get; set; }

        public int UserId { get; set; }
        public User Creator { get; set; }


        public List<GuestWedding> GuestList { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}