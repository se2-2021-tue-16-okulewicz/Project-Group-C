﻿using System;

namespace Backend.DTOs.Dogs
{
    public class GetLostDogDto : GetDogDto
    {
        public LocationDto Location { get; set; }

        public DateTime DateLost { get; set; }

        public bool IsFound { get; set; } = false;

        public int OwnerId { get; set; }
    }
}
