using Domain.DTOs.CompilerDto;
using Domain.DTOs.QuestionGeneratorDto;
using Domain.Responses;

namespace Infrastructure.Services.GeneratorServices;

public interface IGenerateService
{
    Task<Response<string>> GenerateQuestionWithTestsAsync(FilterQuestionDto filterQuestion);
    Task<Response<ResponseCompilation>> TestQuestionAsync( CompilerDto compilerDto);
    Task<Response<GenerateQuestionDto>> GetGenerateQuestionAsync(int id);
}