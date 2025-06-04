using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.DTOs
{
    public class SignatureDto
    {
        [Required]
        public string Signature { get; set; } = string.Empty; // Base64 string
    }
}
