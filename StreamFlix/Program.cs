using StreamFlix.Mappers;
using StreamFlix.Retrievers;
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

builder.Services.AddScoped<IShelvesService, ShelvesService>();
builder.Services.AddScoped<IDataSourceRetriever, TrendingNowRetriever>();
//builder.Services.AddScoped<IDataSourceRetriever, ContinuePlayingRetriever>();
builder.Services.AddScoped<IShelfMapper, HeaderShelfMapper>();
builder.Services.AddScoped<IShelfMapper, ShowsShelfMapper>();

// TODO: set up auth for calling APIs
builder.Services.AddHttpClient<ILayoutService, LayoutService>(client =>
{
    var layoutServiceBaseUrl = builder.Configuration["LayoutService:BaseUrl"];
    client.BaseAddress = new Uri(layoutServiceBaseUrl);
    // TODO: set up a global timeout for all HttpClient instances
    client.Timeout = TimeSpan.FromSeconds(1);
});

builder.Services.AddHttpClient<IRecommendationsService, RecommendationsService>(client =>
{
    var trendingServiceBaseUrl = builder.Configuration["TrendingService:BaseUrl"];
    client.BaseAddress = new Uri(trendingServiceBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(1);
});

builder.Services.AddHttpClient<IVideoLibraryService, VideoLibraryService>(client =>
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