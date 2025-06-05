using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.DTOs
{
    public class EmployeeListQueryDto
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }

        public string? SortDirection { get; set; } // "asc" or "desc"

        public string? Filter { get; set; }
    }
}
