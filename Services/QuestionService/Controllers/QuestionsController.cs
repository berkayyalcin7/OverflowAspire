using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Data;
using QuestionService.DTOs;
using QuestionService.Models;
using System.Security.Claims;

namespace QuestionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController(QuestionDbContext db) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateQuestion(CreateQuestionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var name = User.FindFirstValue("name");

            if (userId is null || name is null)
            {
                return BadRequest("Cannot get user details");
            }

            var question = new Question
            {
                Title = dto.Title,
                Content = dto.Content,
                TagSlugs = dto.Tags,
                AskerId = userId,
                AskerDisplayName = name
            };

            db.Questions.Add(question);

            await db.SaveChangesAsync();

            return Created($"/questions/{question.Id}", question);
        }
    }
}
