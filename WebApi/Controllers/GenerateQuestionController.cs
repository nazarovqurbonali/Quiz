using Domain.DTOs.CompilerDto;
using Domain.DTOs.QuestionGeneratorDto;
using Infrastructure.Services.GeneratorServices;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class GenerateQuestionController:BaseController
{
    private readonly IGenerateService _service;

    public GenerateQuestionController(IGenerateService service)
    {
        _service = service;
    }

    [HttpPost("generate-question")]
    public async Task<IActionResult> GenerateQuestionAsync([FromQuery]FilterQuestionDto filter)
    {
        if (ModelState.IsValid)
        {
            var response = await _service.GenerateQuestionWithTestsAsync(filter);
            return StatusCode(response.StatusCode, response);
        }

        var message = ModelStateErrors();
        return StatusCode(400, message);
    }
    
    [HttpGet("get-generate-questions")]
    public async Task<IActionResult> GenerateQuestionAsync(int id )
    {
        if (ModelState.IsValid)
        {
            var response = await _service.GetGenerateQuestionAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        var message = ModelStateErrors();
        return StatusCode(400, message);
    }
    
    [HttpPost("submissions")]
    public async Task<IActionResult> TestQuestionAsync([FromBody]CompilerDto compilerDto)
    {
        if (ModelState.IsValid)
        {
            var response = await _service.TestQuestionAsync(compilerDto);
            return StatusCode(response.StatusCode, response);
        }
        var message = ModelStateErrors();
        return StatusCode(400,message);
    }
    
}