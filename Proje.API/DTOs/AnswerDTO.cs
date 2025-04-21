namespace Proje.API.DTOs
{
    public class AnswerDTO : BaseDTO
    {
        public int QuestionId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string AnswerText { get; set; }
        public bool IsBySeller { get; set; }
    }

    public class CreateAnswerDTO
    {
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
    }
}