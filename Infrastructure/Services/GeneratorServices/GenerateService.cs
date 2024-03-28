using System.Diagnostics;
using System.Net;
using Domain.DTOs.CompilerDto;
using Domain.DTOs.QuestionDto;
using Domain.DTOs.QuestionGeneratorDto;
using Domain.Entities;
using Domain.Enums;
using Domain.Responses;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.GeneratorServices;

public class GenerateService : IGenerateService
{
    private readonly DataContext _context;
    private readonly ILogger<GenerateService> _logger;

    public GenerateService(DataContext context, ILogger<GenerateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region GenerateQuestionWithTestsAsync

    public async Task<Response<string>> GenerateQuestionWithTestsAsync(FilterQuestionDto? filterQuestion)
    {
        try
        {
            _logger.LogInformation("Start service [ GenerateQuestionWithTestsAsync ] in time: {DateTimeNow} ",
                DateTime.Now);

            if (filterQuestion == null)
                return new Response<string>(HttpStatusCode.BadRequest, "Please fill data correctly");

            var takeQuestions = new List<Question>();

            if (filterQuestion.CountOfEasyTask != 0)
            {
                var easyQuestions = await GetRandomQuestions(LevelQuestion.Easy, filterQuestion.Topic,
                    filterQuestion.CountOfEasyTask);
                takeQuestions.AddRange(easyQuestions);
            }

            if (filterQuestion.CountOfMediumTask != 0)
            {
                var mediumQuestions = await GetRandomQuestions(LevelQuestion.Medium, filterQuestion.Topic,
                    filterQuestion.CountOfMediumTask);
                takeQuestions.AddRange(mediumQuestions);
            }

            if (filterQuestion.CountOfHardTask != 0)
            {
                var hardQuestions = await GetRandomQuestions(LevelQuestion.Hard, filterQuestion.Topic,
                    filterQuestion.CountOfHardTask);
                takeQuestions.AddRange(hardQuestions);
            }

            if (filterQuestion.CountOfSuperHardTask != 0)
            {
                var superHardQuestions = await GetRandomQuestions(LevelQuestion.SuperHard, filterQuestion.Topic,
                    filterQuestion.CountOfSuperHardTask);
                takeQuestions.AddRange(superHardQuestions);
            }

            var newGenerator = new Generator()
            {
                IsActive = true,
                CreateAt = DateTimeOffset.UtcNow,
                UpdateAt = DateTimeOffset.UtcNow,
                FromTime = filterQuestion.FromTime,
                ToTime = filterQuestion.ToTime,
            };

            await _context.Generators.AddAsync(newGenerator);
            await _context.SaveChangesAsync();

            foreach (var question in takeQuestions)
            {
                var newQuesGen = new QuestionGenerator()
                {
                    QuestionId = question.Id,
                    GeneratorId = newGenerator.Id
                };
                await _context.QuestionGenerators.AddAsync(newQuesGen);
            }

            await _context.SaveChangesAsync();


            _logger.LogInformation("Finish service [ GenerateQuestionWithTestsAsync ] in time: {DateTimeNow} ",
                DateTime.Now);
            return new Response<string>($"Successfully generated, with id: [{newGenerator.Id}]");
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {EMessage}", e.Message);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    private async Task<List<Question>> GetRandomQuestions(LevelQuestion level, Topic topic, int count)
    {
        var random = new Random();

        var questions = await _context.Questions
            .Where(q => q.LevelQuestion == level && q.Topic == topic)
            .ToListAsync();

        questions = questions.OrderBy(q => random.Next()).ToList();

        return questions.Take(count).ToList();
    }

    #endregion

    public async Task<Response<ResponseCompilation>> TestQuestionAsync(CompilerDto compilerDto)
    {
        try
        {
            _logger.LogInformation("Starting service: [ TestQuestionAsync ] in time: [ {DateTimeNow} ]", DateTime.Now);

            var question = await _context.QuestionGenerators
                .Include(x => x.Question)
                .ThenInclude(q => q.Variants)
                .FirstOrDefaultAsync(x =>
                    x.Generator != null && x.QuestionId == compilerDto.QuestionId && x.Generator.IsActive);

            if (question == null)
                return new Response<ResponseCompilation>(HttpStatusCode.BadRequest, "Question not found");

            var testsQuestion = question.Question?.Variants;
            if (testsQuestion == null || !testsQuestion.Any())
                return new Response<ResponseCompilation>(HttpStatusCode.OK, "This question has no tests");

            var successTest = 0;

            foreach (var test in testsQuestion)
            {
                var result = await ExecuteCode(compilerDto.Code, test.Input, test.Output);
                if (result.IsSuccess == false)
                {
                    return new Response<ResponseCompilation>(
                        new ResponseCompilation()
                        {
                            Error = result.Error, Id = question.QuestionId, Status = Status.Fail,
                            Time = DateTimeOffset.UtcNow, CountAllTests = testsQuestion.Count, CountSuccessTests = 0
                        });
                }

                successTest++;
            }

            var newSaveTest = new SaveTrial()
            {
                SolveTask = compilerDto.Code,
                QuestionId = compilerDto.QuestionId,
                CreateAt = DateTimeOffset.UtcNow,
                UpdateAt = DateTimeOffset.UtcNow,
                IsSolve = testsQuestion.Count == successTest
            };

            _context.SaveTrials.Add(newSaveTest);
            await _context.SaveChangesAsync();

            if (successTest != testsQuestion.Count)
                return new Response<ResponseCompilation>(new ResponseCompilation()
                {
                    Id = question.QuestionId,
                    Error = null,
                    Time = DateTimeOffset.UtcNow,
                    CountAllTests = testsQuestion.Count,
                    CountSuccessTests = successTest,
                    Status = Status.Fail,
                });

            _logger.LogInformation("Finishing service: [ TestQuestionAsync ] in time: [ {DateTimeNow} ]", DateTime.Now);
            return new Response<ResponseCompilation>(new ResponseCompilation()
            {
                Id = question.QuestionId,
                CountSuccessTests = testsQuestion.Count,
                CountAllTests = testsQuestion.Count,
                Status = Status.Correct,
                Error = null,
                Time = DateTimeOffset.UtcNow
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Error:{EMessage}", e.Message);
            return new Response<ResponseCompilation>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    public async Task<Response<GenerateQuestionDto>> GetGenerateQuestionAsync(int id)
    {
        try
        {
            _logger.LogInformation("Starting service in time: [{DateTimeNow}] ", DateTime.UtcNow);
            var generator = await _context.Generators.FirstOrDefaultAsync(x => x.Id == id);
            if (generator == null) return new Response<GenerateQuestionDto>(HttpStatusCode.BadRequest, "Not Found");

            var result = await (from qg in _context.QuestionGenerators
                join q in _context.Questions on qg.QuestionId equals q.Id
                where qg.GeneratorId == id && qg.Generator.IsActive
                select new GetQuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Topic = q.Topic.ToString(),
                    QuestionType = q.LevelQuestion.ToString()
                }).ToListAsync();

            var response = new GenerateQuestionDto()
            {
                FromTime = generator.FromTime,
                ToTime = generator.ToTime,
                CountOfTasks = result.Count,
                GetQuestion = result
            };
            _logger.LogInformation("Finish service in time: [{DateTimeNow}] ", DateTime.UtcNow);
            return new Response<GenerateQuestionDto>(response);
        }
        catch (Exception e)
        {
            _logger.LogError("Error: [{MessageE}] ", e.Message);
            return new Response<GenerateQuestionDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }


    private async Task<ResponseExecute> ExecuteCode(string code, string? input, string expectedOutput)
    {
        try
        {
            string directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(directory);

            string sourceFile = Path.Combine(directory, "code.cpp");

            var fullCode = code;
            await File.WriteAllTextAsync(sourceFile, fullCode);

            string executableFile = Path.Combine(directory, "code.exe");

            Process compilerProcess = new Process();
            compilerProcess.StartInfo.FileName = "g++"; // Используем g++ для компиляции C++ кода
            compilerProcess.StartInfo.Arguments = $"\"{sourceFile}\" -o \"{executableFile}\"";
            compilerProcess.StartInfo.UseShellExecute = false;
            compilerProcess.StartInfo.RedirectStandardOutput = true;
            compilerProcess.StartInfo.RedirectStandardError = true;

            compilerProcess.Start();

            await compilerProcess.WaitForExitAsync();

            if (compilerProcess.ExitCode != 0)
            {
                string compilationError = await compilerProcess.StandardError.ReadToEndAsync();

                _logger.LogError("Compilation Error, in time [ {DateTimeNow} ]", DateTime.Now);
                Directory.Delete(directory, true);
                var response = new ResponseExecute()
                {
                    Error = compilationError,
                    IsSuccess = false
                };
                return response;
            }

            Process executionProcess = new Process();
            executionProcess.StartInfo.FileName = executableFile;
            executionProcess.StartInfo.UseShellExecute = false;
            executionProcess.StartInfo.RedirectStandardInput = true;
            executionProcess.StartInfo.RedirectStandardOutput = true;
            executionProcess.Start();


            if (!string.IsNullOrEmpty(input))
            {
                await executionProcess.StandardInput.WriteLineAsync(input);
                executionProcess.StandardInput.Close();
            }

            var actualOutput = await executionProcess.StandardOutput.ReadToEndAsync();

            await executionProcess.WaitForExitAsync();

            Directory.Delete(directory, true);


            if (!actualOutput.Equals(expectedOutput))
                return new ResponseExecute() { Error = "Error", IsSuccess = false };

            var result = new ResponseExecute()
            {
                Error = null,
                IsSuccess = true,
            };
            return result;
        }
        catch (Exception e)
        {
            return new ResponseExecute { Error = e.Message, IsSuccess = false };
        }
    }
}



public class ResponseExecute
{
    public string? Error { get; set; }
    public bool IsSuccess { get; set; }
}