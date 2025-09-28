using StreamFlix.Services.Layout;
using StreamFlix.Services.Recommendations;
using StreamFlix.Services.Shelves;
using StreamFlix.Services.VideoLibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ILayoutService, LayoutService>();
builder.Services.AddSingleton<IRecommendationsService, RecommendationsService>();
builder.Services.AddSingleton<IShelvesService, ShelvesService>();
builder.Services.AddSingleton<IVideoLibraryService, VideoLibraryService>();

// TODO: set up auth for calling APIs
builder.Services.AddHttpClient<ILayoutService>(client =>
{
    var layoutServiceBaseUrl = builder.Configuration["LayoutService:BaseUrl"];
    client.BaseAddress = new Uri(layoutServiceBaseUrl);
    // TODO: set up a global timeout for all HttpClient instances
    client.Timeout = TimeSpan.FromSeconds(1);
});

builder.Services.AddHttpClient<IRecommendationsService>(client =>
{
    var trendingServiceBaseUrl = builder.Configuration["TrendingService:BaseUrl"];
    client.BaseAddress = new Uri(trendingServiceBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(1);
});

builder.Services.AddHttpClient<IVideoLibraryService>(client =>
{
    var videoLibraryServiceBaseUrl = builder.Configuration["VideoLibraryService:BaseUrl"];
    client.BaseAddress = new Uri(videoLibraryServiceBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(1);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();