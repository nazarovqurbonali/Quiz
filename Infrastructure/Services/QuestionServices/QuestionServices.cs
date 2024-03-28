using System.Net;
using Domain.DTOs.QuestionDto;
using Domain.DTOs.VariantDto;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.QuestionServices;

public class QuestionServices : IQuestionServices
{
    #region ctor

    private readonly DataContext _context;
    private readonly ILogger<QuestionServices> _logger;

    public QuestionServices(DataContext context, ILogger<QuestionServices> logger)
    {
        _context = context;
        _logger = logger;
    }

    #endregion

    #region GetQuestionsAsync

    public async Task<PagedResponse<List<GetQuestionDto>>> GetQuestionsAsync(QuestionFilter filter)
    {
        try
        {
            _logger.LogInformation("Start service [ GetQuestionsAsync ] in time: [ {DateTimeNow} ],", DateTime.UtcNow);

            var questions = _context.Questions.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Text))
                questions = questions.Where(x => x.Text.ToLower().Contains(filter.Text.ToLower()));
            if (!string.IsNullOrEmpty(filter.LevelQuestion.ToString()))
                questions = questions.Where(x =>
                    x.LevelQuestion == filter.LevelQuestion);
            if (!string.IsNullOrEmpty(filter.Topic.ToString()))
                questions = questions.Where(x =>
                    x.Topic == filter.Topic);

            var response = await questions
                .Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
            var count = await questions.CountAsync();

            var result = new List<GetQuestionDto>();
            foreach (var l in response)
            {
                var newCategory = new GetQuestionDto
                {
                    Id = l.Id,
                    Text = l.Text,
                    QuestionType = l.LevelQuestion.ToString(),
                    Topic = l.Topic.ToString(),
                };
                result.Add(newCategory);
            }

            _logger.LogInformation("Finish service [ GetQuestionsAsync ] in time: [ {DateTimeNow} ],", DateTime.UtcNow);
            return new PagedResponse<List<GetQuestionDto>>(result, filter.PageNumber, filter.PageSize, count);
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {EMessage} , time: [ {DateTimeNow} ],", e.Message, DateTime.UtcNow);
            return new PagedResponse<List<GetQuestionDto>>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region GetQuestionsWithVariantsAsync

    public async Task<Response<List<GetQuestionWithVarianstDto>>> GetQuestionsWithVariantsAsync(QuestionFilter filter)
    {
        try
        {
            _logger.LogInformation("Start service [ GetQuestionsWithVariantsAsync ] in time: [ {DateTimeNow} ],",
                DateTime.UtcNow);

            var questions = _context.Questions.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Text))
                questions = questions.Where(x => x.Text.ToLower().Contains(filter.Text.ToLower()));
            if (!string.IsNullOrEmpty(filter.LevelQuestion.ToString()))
                questions = questions.Where(x =>
                    x.LevelQuestion == filter.LevelQuestion);
            if (!string.IsNullOrEmpty(filter.Topic.ToString()))
                questions = questions.Where(x =>
                    x.Topic == filter.Topic);


            var count = await questions.CountAsync();
            var response = await questions.Where(x => x.Id != 0).Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize).ToListAsync();
            var result = new List<GetQuestionWithVarianstDto>();
            foreach (var q in response)
            {
                var tests = new List<GetVariantDto>();
                var listTests = await _context.Variants.Where(x => x.QuestionId == q.Id).Select(x => x).ToListAsync();
                foreach (var t in listTests)
                {
                    var newTest = new GetVariantDto()
                    {
                        Input = t.Input,
                        Output = t.Output,
                        QuestionId = t.QuestionId,
                        Id = t.Id,
                    };
                    tests.Add(newTest);
                }

                var questionWithTest = new GetQuestionWithVarianstDto()
                {
                    Text = q.Text,
                    QuestionType = q.LevelQuestion.ToString(),
                    Id = q.Id,
                    Tests = tests,
                    Topic = q.Topic.ToString()
                };
                result.Add(questionWithTest);
            }

            _logger.LogInformation("Finish service [ GetQuestionsWithVariantsAsync ] in time: [ {DateTimeNow} ],",
                DateTime.UtcNow);
            return new PagedResponse<List<GetQuestionWithVarianstDto>>(result, filter.PageNumber, filter.PageSize, count);
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {EMessage} , time: [ {DateTimeNow} ],", e.Message, DateTime.UtcNow);
            return new PagedResponse<List<GetQuestionWithVarianstDto>>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region GetVariantByIdAsync

    public async Task<Response<GetVariantDto>> GetVariantByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Start service [ GetVariantByIdAsync ] in time: [ {DateTimeNow} ],", DateTime.UtcNow);
            var existing = await _context.Variants.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return new Response<GetVariantDto>(HttpStatusCode.BadRequest, "Variant not found");
            var response = new GetVariantDto()
            {
                Input = existing.Input,
                Output = existing.Output,
                QuestionId = existing.QuestionId,
                Id = existing.Id
            };

            _logger.LogInformation("Finish service [ GetVariantByIdAsync ] in time: [ {DateTimeNow} ],", DateTime.UtcNow);
            return new Response<GetVariantDto>(response);
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {EMessage} , time: [ {DateTimeNow} ],", e.Message, DateTime.UtcNow);
            return new Response<GetVariantDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region GetQuestionByIdAsync

    public async Task<Response<GetQuestionDto>> GetQuestionByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Start service [ GetQuestionByIdAsync ] in time: [ {DateTimeNow} ],",
                DateTime.UtcNow);

            var existing = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null)
                return new Response<GetQuestionDto>(HttpStatusCode.BadRequest, "Question not found !");
            var response = new GetQuestionDto()
            {
                Text = existing.Text,
                QuestionType = existing.LevelQuestion.ToString(),
                Id = existing.Id,
                Topic = existing.Topic.ToString()
            };

            _logger.LogInformation("Finish service [ GetQuestionByIdAsync ] in time: [ {DateTimeNow} ],",
                DateTime.UtcNow);
            return new Response<GetQuestionDto>(response);
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {EMessage} , time: [ {DateTimeNow} ],", e.Message, DateTime.UtcNow);
            return new Response<GetQuestionDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region GetQuestionWithVariantByIdAsync

    public async Task<Response<GetQuestionWithVarianstDto>> GetQuestionWithVariantByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Start service [ GetQuestionWithVariantByIdAsync ] in time: [ {DateTimeNow} ],",
                DateTime.UtcNow);

            var existingQuestion = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
            if (existingQuestion == null)
                return new Response<GetQuestionWithVarianstDto>(HttpStatusCode.BadRequest, "Question not found !");
            var exitingTest = await _context.Variants
                .Where(x => x.QuestionId == existingQuestion.Id)
                .Select(x => x).ToListAsync();
            var convertTest = new List<GetVariantDto>();
            foreach (var t in exitingTest)
            {
                var newTest = new GetVariantDto()
                {
                    Input = t.Input,
                    Output = t.Output,
                    QuestionId = t.QuestionId,
                    Id = t.Id,
                };
                convertTest.Add(newTest);
            }

            var response = new GetQuestionWithVarianstDto()
            {
                Text = existingQuestion.Text,
                QuestionType = existingQuestion.LevelQuestion.ToString(),
                Id = existingQuestion.Id,
                Tests = convertTest,
                Topic = existingQuestion.Topic.ToString()
            };

            _logger.LogInformation("Finish service [ GetQuestionWithVariantByIdAsync ] in time: [ {DateTimeNow} ],",
                DateTime.UtcNow);
            return new Response<GetQuestionWithVarianstDto>(response);
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {EMessage} , time: [ {DateTimeNow} ],", e.Message, DateTime.UtcNow);
            return new Response<GetQuestionWithVarianstDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region CreateQuestionAsync

    public async Task<Response<string>> CreateQuestionAsync(CreateQuestionDto create)
    {
        try
        {
            _logger.LogInformation("Starting service [ CreateQuestionWithVariantsAsync ] in time: {UtcNow}",
                DateTime.UtcNow);
            var newQuestion = new Question()
            {
                Text = create.Text,
                LevelQuestion = create.LevelQuestion,
                CreateAt = DateTimeOffset.UtcNow,
                UpdateAt = DateTimeOffset.UtcNow,
                Topic = create.Topic
            };
            await _context.Questions.AddAsync(newQuestion);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Finishing service [ CreateQuestionWithVariantsAsync ] in time: {UtcNow}",
                DateTime.UtcNow);
            return new Response<string>($"Successfully created a new question with id: [ {newQuestion.Id} ]");
        }
        catch (Exception e)
        {
            _logger.LogError("Error:{EMessage}", e.Message);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region CreateVariantAsync

    public async Task<Response<string>> CreateVariantAsync(CreateVariantDto create)
    {
        try
        {
            _logger.LogInformation("Starting service [ CreateVariantAsync ] in time: {UtcNow}", DateTime.UtcNow);
            var newQuestion = new Variant()
            {
                Input = create.Input,
                Output = create.Output,
                QuestionId = create.QuestionId,
                CreateAt = DateTimeOffset.UtcNow,
                UpdateAt = DateTimeOffset.UtcNow,
            };
            await _context.Variants.AddAsync(newQuestion);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Finishing service [ CreateVariantAsync ] in time: {UtcNow}", DateTime.UtcNow);
            return new Response<string>($"Successfully created a new test with id: [ {newQuestion.Id} ]");
        }
        catch (Exception e)
        {
            _logger.LogError("Error:{EMessage}", e.Message);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region CreateQuestionWithVariantsAsync

    public async Task<Response<string>> CreateQuestionWithVariantsAsync(CreateQuestionDto question,
        List<CreateQuestionWithVariantsDto> tests)
    {
        try
        {
            _logger.LogInformation("Start service [ CreateQuestionWithVariantsAsync ] in time: [ {DateTimeNow} ],",
                DateTime.UtcNow);


            var newQuestion = new Question()
            {
                Text = question.Text,
                LevelQuestion = question.LevelQuestion,
                CreateAt = DateTimeOffset.UtcNow,
                UpdateAt = DateTimeOffset.UtcNow,
                Topic = question.Topic,
            };
            await _context.Questions.AddAsync(newQuestion);
            await _context.SaveChangesAsync();


            foreach (var t in tests)
            {
                var newTest = new Variant()
                {
                    Input = t.Input,
                    Output = t.Output,
                    QuestionId = newQuestion.Id,
                    CreateAt = DateTimeOffset.UtcNow,
                    UpdateAt = DateTimeOffset.UtcNow
                };
                await _context.Variants.AddRangeAsync(newTest);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Finish service [ CreateQuestionWithVariantsAsync ] in time: [ {DateTimeNow} ],",
                DateTime.UtcNow);
            return new Response<string>($"Successfully created a new question with id: [ {newQuestion.Id} ]");
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {EMessage} , time: [ {DateTimeNow} ],", e.Message, DateTime.UtcNow);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region UpdateQuestionAsync

    public async Task<Response<string>> UpdateQuestionAsync(UpdateQuestionDto update)
    {
        try
        {
            _logger.LogInformation("Starting service [ UpdateQuestionWithTestsAsync ] in time: {UtcNow}",
                DateTime.UtcNow);

            var existing = await _context.Questions.FirstOrDefaultAsync(x => x.Id == update.Id);
            if (existing == null) return new Response<string>(HttpStatusCode.BadRequest, "Question not found");

            existing.LevelQuestion = update.LevelQuestion;
            existing.Text = update.Text;
            existing.UpdateAt = DateTimeOffset.UtcNow;
            existing.Id = update.Id;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Finishing service [ UpdateQuestionWithTestsAsync ] in time: {UtcNow}",
                DateTime.UtcNow);
            return new Response<string>("Successfully updated !");
        }
        catch (Exception e)
        {
            _logger.LogError("Error:{EMessage}", e.Message);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region UpdateVariantAsync

    public async Task<Response<string>> UpdateVariantAsync(UpdateVariantDto update)
    {
        try
        {
            _logger.LogInformation("Starting service [ CreateVariantAsync ] in time: {UtcNow}", DateTime.UtcNow);
            var existing = await _context.Variants.FirstOrDefaultAsync(x => x.Id == update.Id);
            if (existing == null) return new Response<string>(HttpStatusCode.BadRequest, "Variant not found");

            existing.QuestionId = update.QuestionId;
            existing.Input = update.Input;
            existing.Output = update.Output;
            existing.UpdateAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Finishing service [ CreateVariantAsync ] in time: {UtcNow}", DateTime.UtcNow);
            return new Response<string>("Successfully updated !");
        }
        catch (Exception e)
        {
            _logger.LogError("Error:{EMessage}", e.Message);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion


    #region DeleteQuestionAsync

    public async Task<Response<bool>> DeleteQuestionAsync(int id)
    {
        try
        {
            _logger.LogInformation("Start service [ DeleteQuestionAsync ] in time: [ {DateTimeNow} ],",
                DateTime.UtcNow);

            var existing = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return new Response<bool>(HttpStatusCode.BadRequest, "Question not found");
            _context.Questions.Remove(existing);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Finish service [ DeleteQuestionAsync ] in time: [ {DateTimeNow} ],",
                DateTime.UtcNow);
            return new Response<bool>(true);
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {EMessage} , time: [ {DateTimeNow} ],", e.Message, DateTime.UtcNow);
            return new Response<bool>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region DeleteVariantAsync

    public async Task<Response<bool>> DeleteVariantAsync(int id)
    {
        try
        {
            _logger.LogInformation("Start service [ DeleteVariantAsync ] in time: [ {DateTimeNow} ],", DateTime.UtcNow);

            var existing = await _context.Variants.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return new Response<bool>(HttpStatusCode.BadRequest, "Variant not found");
            _context.Variants.Remove(existing);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Finish service [ DeleteVariantAsync ] in time: [ {DateTimeNow} ],", DateTime.UtcNow);
            return new Response<bool>(true);
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {EMessage} , time: [ {DateTimeNow} ],", e.Message, DateTime.UtcNow);
            return new Response<bool>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion
}