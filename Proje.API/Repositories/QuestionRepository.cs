using Microsoft.EntityFrameworkCore;
using Proje.API.Data;
using Proje.API.Models;

namespace Proje.API.Repositories
{
    public class QuestionRepository : GenericRepository<Question>
    {
        public QuestionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Question>> GetQuestionsByProductIdAsync(int productId)
        {
            return await _context.Questions
                .Where(q => q.ProductId == productId && q.IsPublic)
                .Include(q => q.Answers)
                .ToListAsync();
        }

        public async Task<Question> GetQuestionWithAnswersAsync(int questionId)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == questionId);
        }
    }
}