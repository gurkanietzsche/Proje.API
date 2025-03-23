// Dosyayı ResultDTO.cs'den ApiResult.cs olarak yeniden adlandırın
namespace Proje.API.Models
{
    public class ApiResult
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}