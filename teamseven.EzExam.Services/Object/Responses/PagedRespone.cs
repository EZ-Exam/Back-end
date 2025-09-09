using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Responses
{
        public class PagedResponse<T>
        {
            public List<T> Items { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalItems { get; set; }
            public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

            public PagedResponse(List<T> items, int pageNumber, int pageSize, int totalItems)
            {
                Items = items;
                PageNumber = pageNumber;
                PageSize = pageSize;
                TotalItems = totalItems;
            }
        }
}
