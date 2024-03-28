using Infrastructure.Data;
using Infrastructure.Services.GeneratorServices;
using Infrastructure.Services.QuestionServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.RegisterService;

public static class RegisterService
{
    public static void AddRegisterService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(configure =>
            configure.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        
        services.AddScoped<IQuestionServices,QuestionServices>();
        services.AddScoped<IGenerateService, GenerateService>();
    }
}