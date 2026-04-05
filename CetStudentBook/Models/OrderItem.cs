using System.ComponentModel.DataAnnotations;

namespace CetStudentBook.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Range(0.01, 1_000_000)]
        public decimal UnitPrice { get; set; }
    }
}