namespace Proje.API.DTOs
{
    public class ResultDTO
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}