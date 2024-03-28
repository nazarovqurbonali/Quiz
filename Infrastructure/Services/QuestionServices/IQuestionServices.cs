using Domain.DTOs.QuestionDto;
using Domain.DTOs.VariantDto;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Services.QuestionServices;

public interface IQuestionServices
{
       Task<PagedResponse<List<GetQuestionDto>>> GetQuestionsAsync(QuestionFilter filter);
       Task<Response<List<GetQuestionWithVarianstDto>>> GetQuestionsWithVariantsAsync(QuestionFilter filter);
       Task<Response<GetVariantDto>> GetVariantByIdAsync(int id);
       Task<Response<GetQuestionDto>> GetQuestionByIdAsync(int id);
       Task<Response<GetQuestionWithVarianstDto>> GetQuestionWithVariantByIdAsync(int id);
       Task<Response<string>> CreateQuestionAsync(CreateQuestionDto create);
       Task<Response<string>> UpdateQuestionAsync(UpdateQuestionDto update);
       Task<Response<string>> CreateVariantAsync(CreateVariantDto create);
       Task<Response<string>> UpdateVariantAsync(UpdateVariantDto update);
       Task<Response<string>> CreateQuestionWithVariantsAsync(CreateQuestionDto question, List<CreateQuestionWithVariantsDto> tests);
       Task<Response<bool>> DeleteQuestionAsync(int id);
       Task<Response<bool>> DeleteVariantAsync(int id);


      


}