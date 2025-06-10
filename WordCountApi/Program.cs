using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WordCountApi.Data;
using WordCountApi.Services.Background.WordCountApi.Services.Background;
using WordCountApi.Services.Implementations;
using WordCountApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Create and open SQLite in-memory connection (keep alive across scope)
var connection = new SqliteConnection("DataSource=:memory:");
connection.Open();

// Register EF Core with shared connection
builder.Services.AddDbContext<WordDbContext>(options =>
    options.UseSqlite(connection));

// Register services & patterns
builder.Services.AddScoped<IWordRepository, WordRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IWordCountService, WordCountService>();

// Register background service
builder.Services.AddHostedService<WordProcessingService>();

// Versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // e.g., v1, v1.0
    options.SubstituteApiVersionInUrl = true;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddControllers();

var app = builder.Build();

// Migrate DB schema
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WordDbContext>();
    db.Database.EnsureCreated(); //  Required for in-memory schema
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
