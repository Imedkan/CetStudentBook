using System.ComponentModel.DataAnnotations;

namespace CetStudentBook.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 1_000_000)]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}