namespace Proje.API.DTOs
{
    public class OrderCreateDTO
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; } = "Türkiye";
        public string PaymentMethod { get; set; } = "Kapıda Ödeme";
    }

    public class OrderStatusUpdateDTO
    {
        public string Status { get; set; }
    }
}