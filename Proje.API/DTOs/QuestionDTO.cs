namespace Proje.API.DTOs
{
    public class QuestionDTO : BaseDTO
    {
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string QuestionText { get; set; }
        public bool IsAnswered { get; set; }
        public List<AnswerDTO> Answers { get; set; }
    }

    public class CreateQuestionDTO
    {
        public int ProductId { get; set; }
        public string QuestionText { get; set; }
    }
}