using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.DTOs
{
    public record ItemDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public double Price { get; init; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
