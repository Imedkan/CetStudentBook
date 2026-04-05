using System.ComponentModel.DataAnnotations;

namespace CetStudentBook.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Range(0.01, 1_000_000)]
        public decimal UnitPrice { get; set; }
    }
}