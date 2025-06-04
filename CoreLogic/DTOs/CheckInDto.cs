using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.DTOs
{
    public class CheckInDto
    {
        public DateTime? CheckInTime { get; set; } // Optional, defaults to current time
    }
}
