using Contracts;
using FastExpressionCompiler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.DTOs;
using QuestionService.Models;
using QuestionService.Services;
using System.Reflection;
using System.Security.Claims;
using Wolverine;

namespace QuestionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController(QuestionDbContext db, IMessageBus bus, TagService tagService) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateQuestion(CreateQuestionDto dto)
        {

            //var validTags = await db.Tags.Where(x => dto.Tags.Contains(x.Slug)).ToListAsync();

            //var missing = dto.Tags.Except(validTags.Select(x => x.Slug).ToList());

            //if (missing.Count() > 0)
            //{
            //    return BadRequest($"Invalid tags : {string.Join(", ", missing)}");
            //}

            if (!await tagService.AreTagsValidAsync(dto.Tags))
            {
                return BadRequest($"Invalid tags");
            }

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

            // Using Transaction for crash.
            await bus.PublishAsync(new QuestionCreated(question.Id, question.Title, question.Content, question.CreatedAt, question.TagSlugs));


            return Created($"/questions/{question.Id}", question);
        }


        [HttpGet]
        public async Task<ActionResult<List<Question>>> GetQuestions(string? tag)
        {
            var query = db.Questions.AsQueryable();

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(x => x.TagSlugs.Contains(tag));
            }

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(string id)
        {
            var question = await db.Questions.Include(x=>x.Answer).FirstOrDefaultAsync(x=>x.Id==id);

            if (question is null) return NotFound();

            // o sırada veriyi çekerken belirli property üzerinde update işlemi
            await db.Questions.Where(x => x.Id == id).ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ViewCount, x => x.ViewCount + 1));

            return question;

        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateQuestion(string id, CreateQuestionDto dto)
        {
            var question = await db.Questions.FindAsync(id);

            if (question is null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != question.AskerId)
            {
                return Forbid();
            }

            //var validTags = await db.Tags.Where(x => dto.Tags.Contains(x.Slug)).ToListAsync();

            //var missing = dto.Tags.Except(validTags.Select(x => x.Slug).ToList());

            //if (missing.Count() > 0)
            //{
            //    return BadRequest($"Invalid tags : {string.Join(", ", missing)}");
            //}

            if (!await tagService.AreTagsValidAsync(dto.Tags))
            {
                return BadRequest($"Invalid tags");
            }

            question.Title = dto.Title;
            question.Content = dto.Content;
            question.TagSlugs = dto.Tags;

            question.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            await bus.PublishAsync(new QuestionUpdated(question.Id,
                question.Title,
                question.Content,
                question.TagSlugs.AsArray()));

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(string id)
        {
            var question = await db.Questions.FindAsync(id);

            if (question is null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != question.AskerId)
            {
                return Forbid();
            }

            db.Questions.Remove(question);

            await bus.PublishAsync(new QuestionDeleted(question.Id));

            await db.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpPost("{questionId}/answers")]
        public async Task<IActionResult> CreateAnswer(CreateAnswerDto dto, string questionId)
        {
            var question = await db.Questions.FindAsync(questionId);

            var name = User.FindFirstValue("name");

            if (question is null) return NotFound();

            if (name is null)
            {
                return BadRequest("Cannot get user details");
            }

            var answer = new Answer
            {
                Content = dto.Content,
                QuestionId = questionId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
                UserDisplayName = User.FindFirstValue("name") ?? ""
            };

            question.Answer.Add(answer);
            question.AnswerCount++;

            await db.SaveChangesAsync();

            await bus.PublishAsync(new AnswerCountUpdated(question.Id, question.AnswerCount));

            return Created($"/questions/{questionId}/answers", answer);
        }

        [Authorize]
        [HttpPut("{questionId}/answers/{answerId}")]
        public async Task<IActionResult> UpdateAnswer(string questionId, string answerId, CreateAnswerDto dto)
        {
            var answer = await db.Answers.FindAsync(answerId);

            if (answer is null || answer.QuestionId != questionId) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != answer.UserId)
            {
                return Forbid();
            }
            answer.Content = dto.Content;

            answer.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return NoContent();

        }

        [Authorize]
        [HttpDelete("{questionId}/answers/{answerId}")]
        public async Task<IActionResult> DeleteAnswer(string questionId, string answerId)
        {
            var answer = await db.Answers.FindAsync(answerId);

            if (answer is null || answer.QuestionId != questionId) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != answer.UserId)
            {
                return Forbid();
            }
            var question = await db.Questions.FindAsync(questionId);

            if (question is not null)
            {
                question.AnswerCount--;
            }

            db.Answers.Remove(answer);

            await db.SaveChangesAsync();

            await bus.PublishAsync(new AnswerCountUpdated(question.Id, question.AnswerCount));

            return NoContent();

        }

        [Authorize]
        [HttpPost("{questionId}/answers/{answerId}/accept")]
        public async Task<IActionResult> AcceptAnswer(string questionId, string answerId)
        {
            var question = await db.Questions.Include(x => x.Answer).FirstOrDefaultAsync(x => x.Id == questionId);

            if (question is null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != question.AskerId)
            {
                return Forbid();
            }
            var answer = question.Answer.FirstOrDefault(x => x.Id == answerId);

            if (answer is null) return NotFound();

            if (answer.QuestionId != questionId || question.HasAcceptedAnswer) 
            {
                return BadRequest();
            }

            answer.Accepted = true;
            question.HasAcceptedAnswer = true;

            await db.SaveChangesAsync();

            await bus.PublishAsync(new AnswerAccepted(question.Id));

            return NoContent();
        }

        [HttpGet("errors")]
        public ActionResult GetErrorResponses(int code)
        {

            ModelState.AddModelError("Problem one", "Validation problem one");
            ModelState.AddModelError("Problem two", "Validation problem two");

            return code switch
            {
                400 => BadRequest("This is a bad request example."),
                401 => Unauthorized("This is an unauthorized example."),
                403 => StatusCode(StatusCodes.Status403Forbidden, "This is a forbidden example."),
                404 => NotFound("This is a not found example."),
                500 => StatusCode(StatusCodes.Status500InternalServerError, "This is an internal server error example."),
                _ => ValidationProblem(ModelState)
            };
        }


    }
}
