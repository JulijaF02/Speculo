using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Speculo.Identity.Data;
using Speculo.Identity.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Database ---
// Identity Service has its OWN database — separate from the Tracking Service
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection")));

// --- Services ---
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<JwtTokenService>();

// --- Kafka Producer ---
// Used to publish UserRegisteredEvent when a new user signs up
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

// --- CORS ---
// Allow the React frontend to call this service
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

var app = builder.Build();

// Apply pending migrations on startup (development convenience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowFrontend");
app.MapControllers();

// Identity Service runs on port 5001 (Tracking Service runs on 5000)
app.Run();
