namespace Proje.API.DTOs
{
    public class OrderDTO : BaseDTO
    {
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        // Adres bilgileri
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        // Ödeme bilgileri
        public string PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }

        public List<OrderItemDTO> OrderItems { get; set; }
    }
}