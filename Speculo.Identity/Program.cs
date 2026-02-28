using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Speculo.Identity.Data;
using Speculo.Identity.Middleware;
using Speculo.Identity.Services;

var builder = WebApplication.CreateBuilder(args);

// Database — Identity Service has its own database
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection")));

// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<JwtTokenService>();

// Kafka producer — publishes UserRegisteredEvent on signup
builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
        Acks = Acks.Leader,
        MessageSendMaxRetries = 3
    };
    return new ProducerBuilder<string, string>(config).Build();
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration["Frontend:Url"] ?? "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();

