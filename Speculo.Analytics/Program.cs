using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;
using Speculo.Analytics.Middleware;
using Speculo.Analytics.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

// ─── MongoDB ───────────────────────────────────────────
// The Analytics Service uses MongoDB for read projections (denormalized data).
// Each user has one DashboardProjection document with pre-computed aggregates.
var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"] ?? "mongodb://localhost:27017";
var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase(
    builder.Configuration["MongoDB:DatabaseName"] ?? "speculo_analytics");
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

// ─── Redis ─────────────────────────────────────────────
// Cache-aside pattern: dashboard queries check Redis first, fall back to MongoDB.
// The Kafka consumer invalidates the cache when new events are processed.
var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnectionString));

// ─── Application Services ──────────────────────────────
builder.Services.AddSingleton<ProjectionService>();
builder.Services.AddHostedService<KafkaConsumerService>();

// ─── JWT Authentication ────────────────────────────────
// Same JWT settings as other services — tokens issued by Identity Service work here too.
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT secret not configured. Set Jwt:Secret in appsettings or environment variables.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Speculo.Identity",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "Speculo",
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ─── Health Checks ─────────────────────────────────────
builder.Services.AddHealthChecks();

// ─── CORS ──────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration["Frontend:Url"] ?? "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ─── Swagger ───────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Create MongoDB indexes on startup
await Speculo.Analytics.Configuration.MongoDbIndexes.EnsureIndexesAsync(mongoDatabase);

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
