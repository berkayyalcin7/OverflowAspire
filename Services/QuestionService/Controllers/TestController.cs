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
    [Route("[controller]")]
    [ApiController]
    public class TestController() : ControllerBase
    {
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

        [Authorize]
        [HttpGet("auth")]
        public ActionResult TestAuth()
        {
            var user = User.FindFirstValue("name");

            return Ok($"{user} has been authorized");
        }
    }
}
