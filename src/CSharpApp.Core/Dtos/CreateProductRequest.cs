using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Dtos
{
    public sealed class CreateProductRequest
    {
        public string? Title { get; set; }
        public int? Price { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public List<string> Images { get; set; } = [];
    }
}
