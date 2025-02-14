﻿using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Dogs
{
    public class PictureDog
    {
        [Required]
        [MaxLength(150)]
        public string FileName { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FileType { get; set; }

        [Required]
        public byte[] Data { get; set; }

        public int DogId { get; set; }

        public override string ToString() => FileName;

    }
}
