using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalEmployees { get; set; }
        public int PresentToday { get; set; }
        public int NonPresentToday { get; set; }
    }
}
