using StreamFlix.Services.Layout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ILayoutService>(client =>
{
    var layoutServiceBaseUrl = builder.Configuration["LayoutService:BaseUrl"];
    client.BaseAddress = new Uri(layoutServiceBaseUrl);

    client.Timeout = TimeSpan.FromSeconds(1);

    // 4. Add default request headers (e.g., for authentication or content type)
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

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
