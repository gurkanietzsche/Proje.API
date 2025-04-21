namespace Proje.API.Models
{
    public class Question : BaseEntity
    {
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public string QuestionText { get; set; }
        public bool IsAnswered { get; set; } = false;
        public bool IsPublic { get; set; } = true; // Yayınlanıp yayınlanmayacağı

        // İlişkiler
        public virtual Product Product { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
    }
}