using System.ComponentModel.DataAnnotations;

namespace StorageApi.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 200000)]
        public int Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;
        public string Shelf { get; set; } = string.Empty;
        public int Count { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
