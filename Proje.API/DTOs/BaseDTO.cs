namespace Proje.API.DTOs
{
    public class BaseDTO
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}