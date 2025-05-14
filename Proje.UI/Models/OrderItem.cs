using Proje.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class OrderItem
{
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    [Required]
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalıdır.")]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    // İlişkiler
    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }

    [NotMapped]
    public decimal Subtotal => Quantity * UnitPrice;
}
}