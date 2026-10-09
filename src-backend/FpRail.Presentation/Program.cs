var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowTauri", policy =>
    {
        policy.AllowAnyOrigin() // Для PoC можно разрешить всё
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5042); // Теперь он ВСЕГДА будет слушать http://localhost:5042
});

builder.Services.AddControllers()
    .AddApplicationPart(typeof(FPRail.Presentation.Controllers.TramPointsController).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var endpointDataSource = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Routing.EndpointDataSource>();
    Console.WriteLine("=== ЗАРЕГИСТРИРОВАННЫЕ ЭНДПОИНТЫ .NET ===");
    foreach (var endpoint in endpointDataSource.Endpoints)
    {
        Console.WriteLine($"-> {endpoint.DisplayName}");
    }
    Console.WriteLine("========================================");
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
