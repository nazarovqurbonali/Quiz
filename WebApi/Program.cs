using Infrastructure.RegisterService;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// connection to database && dependency injection
builder.Services.AddRegisterService(builder.Configuration);
builder.Services.AddCors();


var app = builder.Build();


app.UseCors(
    corsPolicyBuilder => corsPolicyBuilder.WithOrigins("http://127.0.0.1:5500", "http://localhost:3000","https://localhost:3000")
        .SetIsOriginAllowed(_ => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
