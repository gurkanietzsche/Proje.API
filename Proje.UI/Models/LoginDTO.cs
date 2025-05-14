using System.ComponentModel.DataAnnotations;

namespace Proje.UI.Models.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Password { get; set; }
    }
}