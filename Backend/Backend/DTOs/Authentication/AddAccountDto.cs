﻿using Backend.Models.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Authentication
{
    public class AddAccountDto
    {
        [Required(ErrorMessage = "User Name is required")]
        [MaxLength(256)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8)] 
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(20)]
        [MinLength(8)]
        public string PhoneNumber { get; set; }

        public string AccountRole { get; set; } = AccountRoles.Regular;
    }
}
