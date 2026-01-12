using Common;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using QuestionService.Data;
using QuestionService.Services;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// Metrics , Telemetry
builder.AddServiceDefaults();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<TagService>();


await builder.UseWolverineWithRabbitMqAsync(builder.Configuration, opts =>
{
    opts.PublishAllMessages().ToRabbitExchange("questions");
    opts.ApplicationAssembly = typeof(Program).Assembly;
});

builder.Services.AddKeyCloakAuthentication();


// OLD Config
//builder.Host.UseWolverine(opts =>
//{
//    opts.UseRabbitMqUsingNamedConnection("messaging").AutoProvision();
//    opts.PublishAllMessages().ToRabbitExchange("questions");
//});



//// Keycloak Authentication - Common'a taşındı
//builder.Services
//    .AddAuthentication()
//    .AddKeycloakJwtBearer(serviceName: "keycloak", realm: "overflow", options =>
//    {
//        options.RequireHttpsMetadata = false;
//        options.Audience = "overflow";
//        // Bunu eklemediğimizde Invalid issuer hatası alıyoruz. 
//        options.Authority = "http://keycloak:6001/realms/overflow";
//    });

builder.AddNpgsqlDbContext<QuestionDbContext>("questionDb");



var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.MapDefaultEndpoints();


using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<QuestionDbContext>();
    await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating or initializing the database.");
    throw;
}



app.Run();
