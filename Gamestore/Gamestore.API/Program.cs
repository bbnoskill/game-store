using Gamestore.API.Interfaces;
using Gamestore.API.Middleware;
using Gamestore.API.Services;
using Gamestore.BLL.Filtering;
using Gamestore.BLL.Filtering.Implementations;
using Gamestore.BLL.Filtering.Interfaces;
using Gamestore.BLL.Interfaces;
using Gamestore.BLL.Services;
using Gamestore.DAL.Data;
using Gamestore.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GamestoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IFileLoggerService, FileLoggerService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IPlatformService, PlatformService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<IGameFilterStep, GenreFilterStep>();
builder.Services.AddScoped<IGameFilterStep, PlatformFilterStep>();
builder.Services.AddScoped<IGameFilterStep, PublisherFilterStep>();
builder.Services.AddScoped<IGameFilterStep, PriceFilterStep>();
builder.Services.AddScoped<IGameFilterStep, PublishDateFilterStep>();
builder.Services.AddScoped<IGameFilterStep, NameFilterStep>();
builder.Services.AddScoped<IGameFilterStep, SortStep>();
builder.Services.AddScoped<GameFilterPipeline>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("x-total-number-of-games");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseMiddleware<ExceptionHandling>();
app.UseMiddleware<RequestLogging>();
app.UseMiddleware<TotalGamesCount>();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();