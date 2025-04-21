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
    public class QuestionController : ControllerBase
    {
        private readonly QuestionRepository _questionRepository;
        private readonly ProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ResultDTO _result;

        public QuestionController(
            QuestionRepository questionRepository,
            ProductRepository productRepository,
            IMapper mapper)
        {
            _questionRepository = questionRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _result = new ResultDTO();
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetProductQuestions(int productId)
        {
            var questions = await _questionRepository.GetQuestionsByProductIdAsync(productId);
            var questionDtos = _mapper.Map<IEnumerable<QuestionDTO>>(questions);
            return Ok(questionDtos);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResultDTO>> CreateQuestion(CreateQuestionDTO questionDto)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz soru bilgileri";
                return BadRequest(_result);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bilgisi bulunamadı";
                return BadRequest(_result);
            }

            var product = await _productRepository.GetByIdAsync(questionDto.ProductId);
            if (product == null)
            {
                _result.Status = false;
                _result.Message = "Ürün bulunamadı";
                return NotFound(_result);
            }

            var question = new Question
            {
                ProductId = questionDto.ProductId,
                UserId = userId,
                QuestionText = questionDto.QuestionText,
                IsAnswered = false,
                IsPublic = true
            };

            await _questionRepository.AddAsync(question);
            await _questionRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Soru başarıyla eklendi";
            return Ok(_result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{questionId}/visibility")]
        public async Task<ActionResult<ResultDTO>> ToggleQuestionVisibility(int questionId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
            {
                _result.Status = false;
                _result.Message = "Soru bulunamadı";
                return NotFound(_result);
            }

            question.IsPublic = !question.IsPublic;
            await _questionRepository.UpdateAsync(question);
            await _questionRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = question.IsPublic ? "Soru görünür hale getirildi" : "Soru gizlendi";
            return Ok(_result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{questionId}")]
        public async Task<ActionResult<ResultDTO>> DeleteQuestion(int questionId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
            {
                _result.Status = false;
                _result.Message = "Soru bulunamadı";
                return NotFound(_result);
            }

            await _questionRepository.DeleteAsync(questionId);
            await _questionRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Soru başarıyla silindi";
            return Ok(_result);
        }
    }
}