using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace idolapi.DB.DTOs
{
    public class PageParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchName { get; set; } = "";

    }
}