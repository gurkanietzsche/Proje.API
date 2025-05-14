using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proje.Web.Models
{
    public class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Display(Name = "Durum")]
        public string Status { get; set; } = "Beklemede"; // Beklemede, İşleniyor, Kargoya Verildi, Teslim Edildi, İptal Edildi

        // Adres bilgileri
        [Required(ErrorMessage = "Adres zorunludur.")]
        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Şehir zorunludur.")]
        [Display(Name = "Şehir")]
        public string City { get; set; }

        [Required(ErrorMessage = "Posta kodu zorunludur.")]
        [Display(Name = "Posta Kodu")]
        public string PostalCode { get; set; }

        [Display(Name = "Ülke")]
        public string Country { get; set; } = "Türkiye";

        // Ödeme bilgileri
        [Required(ErrorMessage = "Ödeme yöntemi zorunludur.")]
        [Display(Name = "Ödeme Yöntemi")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Ödendi")]
        public bool IsPaid { get; set; } = false;

        // İlişkiler
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }