using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Domain.Core.Model
{
    public class PagedResult<T> where T : class 
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalPages { get; set; }
        public int TotalCount { get; set; } 
    }
}
