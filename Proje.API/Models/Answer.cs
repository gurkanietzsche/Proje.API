namespace Proje.API.Models
{
    public class Answer : BaseEntity
    {
        public int QuestionId { get; set; }
        public string UserId { get; set; }
        public string AnswerText { get; set; }
        public bool IsBySeller { get; set; } = false; // Satıcı cevabı mı?

        // İlişkiler
        public virtual Question Question { get; set; }
    }
}