using Domain.DTOs.QuestionDto;
using Domain.DTOs.VariantDto;
using Domain.Filters;
using Infrastructure.Services.QuestionServices;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class QuestionController : BaseController
{
    private readonly IQuestionServices _services;

    public QuestionController(IQuestionServices services)
    {
        _services = services;
    }

    [HttpGet("get-questions")]
    public async Task<IActionResult> GetQuestionsAsync([FromQuery] QuestionFilter filter)
    {
        var response = await _services.GetQuestionsAsync(filter);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("get-questions-with-tests")]
    public async Task<IActionResult> GetQuestionsWithTestsAsync([FromQuery] QuestionFilter filter)
    {
        var response = await _services.GetQuestionsWithVariantsAsync(filter);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("get-test-by-id")]
    public async Task<IActionResult> GetTestByIdAsync([FromQuery] int id)
    {
        var response = await _services.GetVariantByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("get-questions-by-id")]
    public async Task<IActionResult> GetQuestionByIdAsync([FromQuery] int id)
    {
        var response = await _services.GetQuestionByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("get-question-with-tests-by-id")]
    public async Task<IActionResult> GetQuestionWithTestByIdAsync([FromQuery] int id)
    {
        var response = await _services.GetQuestionWithVariantByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("create-question")]
    public async Task<IActionResult> CreateQuestionAsync([FromBody] CreateQuestionDto create)
    {
        if (ModelState.IsValid)
        {
            var response = await _services.CreateQuestionAsync(create);
            return StatusCode(response.StatusCode, response);
        }

        return BadRequest();
    }

    [HttpPost("create-test")]
    public async Task<IActionResult> CreateTestAsync([FromBody] CreateVariantDto create)
    {
        if (ModelState.IsValid)
        {
            var response = await _services.CreateVariantAsync(create);
            return StatusCode(response.StatusCode, response);
        }

        return BadRequest();
    }

    [HttpPost("create-question-with-tests")]
    public async Task<IActionResult> CreateQuestionWithTestsAsync([FromBody] List<CreateQuestionWithVariantsDto> tests,
        CreateQuestionDto question)
    {
        if (ModelState.IsValid)
        {
            var response = await _services.CreateQuestionWithVariantsAsync(question, tests);
            return StatusCode(response.StatusCode, response);
        }

        return BadRequest();
    }

    [HttpPut("update-question")]
    public async Task<IActionResult> UpdateQuestionAsync([FromBody] UpdateQuestionDto update)
    {
        if (ModelState.IsValid)
        {
            var response = await _services.UpdateQuestionAsync(update);
            return StatusCode(response.StatusCode, response);
        }

        var message = ModelStateErrors();
        return StatusCode(400, message);
    }

    [HttpPut("update-test")]
    public async Task<IActionResult> UpdateTestAsync([FromBody] UpdateVariantDto update)
    {
        if (ModelState.IsValid)
        {
            var response = await _services.UpdateVariantAsync(update);
            return StatusCode(response.StatusCode, response);
        }

        return BadRequest();
    }


    [HttpDelete("delete-question")]
    public async Task<IActionResult> DeleteQuestionAsync([FromBody] int id)
    {
        var response = await _services.DeleteQuestionAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("delete-test")]
    public async Task<IActionResult> DeleteTestAsync([FromBody] int id)
    {
        var response = await _services.DeleteVariantAsync(id);
        return StatusCode(response.StatusCode, response);
    }
}