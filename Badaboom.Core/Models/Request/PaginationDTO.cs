using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badaboom.Core.Models.Request
{
    public class PaginationDTO
    {
        public int page { get; set; } = 1;
        public int quantityPerPage { get; set; } = 10;
    }
}
