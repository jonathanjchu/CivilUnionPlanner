using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class LoginUser
    {
        [Display(Name="Email")]
        [Required]
        public string LoginEmail { get; set; }

        [Display(Name="Password")]
        [DataType(DataType.Password)]
        [Required]
        public string LoginPassword { get; set; }
    }
}