using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proje.API.Models
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Beklemede"; // Beklemede, İşleniyor, Kargoya Verildi, Teslim Edildi, İptal Edildi

        // Adres bilgileri
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; } = "Türkiye";

        // Ödeme bilgileri
        public string PaymentMethod { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; }

        // İlişkiler
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}