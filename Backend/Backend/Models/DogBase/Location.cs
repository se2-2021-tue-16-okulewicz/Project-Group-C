﻿using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DogBase
{
    public class Location
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(100)]
        public string District { get; set; }

    }
}
