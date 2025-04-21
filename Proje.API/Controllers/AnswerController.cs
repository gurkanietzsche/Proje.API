using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proje.API.DTOs;
using Proje.API.Models;
using Proje.API.Repositories;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Proje.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly AnswerRepository _answerRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly IMapper _mapper;
        private readonly ResultDTO _result;

        public AnswerController(
            AnswerRepository answerRepository,
            QuestionRepository questionRepository,
            IMapper mapper)
        {
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
            _mapper = mapper;
            _result = new ResultDTO();
        }

        [HttpGet("question/{questionId}")]
        public async Task<ActionResult<IEnumerable<AnswerDTO>>> GetQuestionAnswers(int questionId)
        {
            var answers = await _answerRepository.GetAnswersByQuestionIdAsync(questionId);
            var answerDtos = _mapper.Map<IEnumerable<AnswerDTO>>(answers);
            return Ok(answerDtos);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResultDTO>> CreateAnswer(CreateAnswerDTO answerDto)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz cevap bilgileri";
                return BadRequest(_result);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bilgisi bulunamadı";
                return BadRequest(_result);
            }

            var question = await _questionRepository.GetByIdAsync(answerDto.QuestionId);
            if (question == null)
            {
                _result.Status = false;
                _result.Message = "Soru bulunamadı";
                return NotFound(_result);
            }

            // Kullanıcı rolünü kontrol edip, satıcı mı yoksa normal kullanıcı mı olduğunu belirleyebilirsiniz
            var isSeller = User.IsInRole("Admin") || User.IsInRole("Seller"); // Örnek, gerçek uygulamada değişebilir

            var answer = new Answer
            {
                QuestionId = answerDto.QuestionId,
                UserId = userId,
                AnswerText = answerDto.AnswerText,
                IsBySeller = isSeller
            };

            await _answerRepository.AddAsync(answer);
            await _answerRepository.SaveChangesAsync();

            // Soru durumunu "cevaplandı" olarak güncelle
            question.IsAnswered = true;
            await _questionRepository.UpdateAsync(question);
            await _questionRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Cevap başarıyla eklendi";
            return Ok(_result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{answerId}")]
        public async Task<ActionResult<ResultDTO>> DeleteAnswer(int answerId)
        {
            var answer = await _answerRepository.GetByIdAsync(answerId);
            if (answer == null)
            {
                _result.Status = false;
                _result.Message = "Cevap bulunamadı";
                return NotFound(_result);
            }

            await _answerRepository.DeleteAsync(answerId);
            await _answerRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Cevap başarıyla silindi";
            return Ok(_result);
        }
    }
}