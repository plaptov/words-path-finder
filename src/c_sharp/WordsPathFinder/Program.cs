using WordsPathFinder.DAL;
using WordsPathFinder.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<WordsPathFinderContext>();
builder.Services.AddHttpClient<GraphCache>(client =>
    client.BaseAddress = new Uri("https://raw.githubusercontent.com/plaptov/words-path-finder/main/dict/"));
builder.Services.AddSingleton<GraphCache>();
builder.Services.AddScoped<IPathFinderService, PathFinderService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
