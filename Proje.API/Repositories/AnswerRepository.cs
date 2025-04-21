using Microsoft.EntityFrameworkCore;
using Proje.API.Data;
using Proje.API.Models;

namespace Proje.API.Repositories
{
    public class AnswerRepository : GenericRepository<Answer>
    {
        public AnswerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId)
        {
            return await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
        }
    }
}