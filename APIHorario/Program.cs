using APIHorario.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "APIHorario",
        Description = "APIHorario - Testando Output Cache middleware com Minimal APIs",
        Version = "v1"
    });
});

builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(5);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.MapGet("/nocache", () =>
{
    var result = new Resultado() { Mensagem = "Teste sem cache" };
    app.Logger.LogInformation($"{result.HorarioAtual} {result.Mensagem}");
    return result;
}).WithOpenApi();

app.MapGet("/cache", () =>
{
    var result = new Resultado() { Mensagem = "Teste com cache" };
    app.Logger.LogInformation($"{result.HorarioAtual} {result.Mensagem}");
    return result;
}).WithOpenApi().CacheOutput();

app.MapGet("/cachequerystring", (string valorTeste) =>
{
    var result = new Resultado()
    {
        Mensagem = $"Teste com cache | Query string: {nameof(valorTeste)} = {valorTeste}"
    };
    app.Logger.LogInformation($"{result.HorarioAtual} {result.Mensagem}");
    return result;
})
.WithOpenApi()
.CacheOutput(policy =>
{
    policy.SetVaryByQuery(["valorTeste"]);
    policy.Expire(TimeSpan.FromSeconds(15));
});

app.UseOutputCache();

app.Run();