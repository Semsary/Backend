﻿using System.ComponentModel.DataAnnotations;

namespace semsary_backend.DTO
{
    public class RateDTO
    {
        [Required]
        public byte StarsNumber { get; set; }
    }
}
