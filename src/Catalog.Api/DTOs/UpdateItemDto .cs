using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.DTOs
{
    public record UpdateItemDto
    {
        [Required]
        public string Name { get; init; }

        [Required]
        [Range(1, 10000)]
        public double Price { get; init; }
    }
}
