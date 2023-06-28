using System;

namespace Catalog.Api.Entities
{
    public record Item
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public double Price { get; init; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}