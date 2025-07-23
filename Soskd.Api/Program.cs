// Soskd.Api/Program.cs - ADD "resumes" to subdirectories array
using Microsoft.EntityFrameworkCore;
using Soskd.Infrastructure.Data;
using Soskd.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    });
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// FileService registration with proper logging and WebRootPath handling
builder.Services.AddScoped<IFileService>(provider =>
{
    var environment = provider.GetRequiredService<IWebHostEnvironment>();
    var logger = provider.GetRequiredService<ILogger<FileService>>();

    // Ensure WebRootPath exists
    var webRootPath = environment.WebRootPath;
    if (string.IsNullOrEmpty(webRootPath))
    {
        // Fallback: create wwwroot in content root if it doesn't exist
        webRootPath = Path.Combine(environment.ContentRootPath, "wwwroot");
        if (!Directory.Exists(webRootPath))
        {
            Directory.CreateDirectory(webRootPath);
            logger.LogInformation("Created wwwroot directory at: {WebRootPath}", webRootPath);
        }
    }

    return new FileService(webRootPath, logger);
});
// *** UPay Service Registration ***
builder.Services.AddHttpClient<IUpayService, UpayService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<IUpayService, UpayService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:8080",
                "http://localhost:5173",
                "http://127.0.0.1:5001"
            );
        }
        else
        {
            policy.WithOrigins(
                "http://95.46.96.145",
                "https://95.46.96.145",
                "http://sos.bsstudio.uz",
                "https://sos.bsstudio.uz",
                "http://127.0.0.1:5001"
            );
        }
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure static file serving for uploads
app.UseStaticFiles();

app.UseCors("AllowVueApp");
app.UseAuthorization();
app.MapControllers();

// Create upload directories on startup
var environment = app.Services.GetRequiredService<IWebHostEnvironment>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    var webRootPath = environment.WebRootPath;
    if (string.IsNullOrEmpty(webRootPath))
    {
        webRootPath = Path.Combine(environment.ContentRootPath, "wwwroot");
    }

    logger.LogInformation("WebRootPath: {WebRootPath}", webRootPath);

    // Create uploads directory structure
    var uploadsPath = Path.Combine(webRootPath, "uploads");
    if (!Directory.Exists(uploadsPath))
    {
        Directory.CreateDirectory(uploadsPath);
        logger.LogInformation("Created uploads directory: {UploadsPath}", uploadsPath);
    }

    // Create subdirectories for different content types INCLUDING resumes
    var subdirectories = new[] { "news", "sponsors", "documents", "media", "general", "resumes" };
    foreach (var subdir in subdirectories)
    {
        var subdirPath = Path.Combine(uploadsPath, subdir);
        if (!Directory.Exists(subdirPath))
        {
            Directory.CreateDirectory(subdirPath);
            logger.LogInformation("Created subdirectory: {SubdirPath}", subdirPath);
        }
    }

    logger.LogInformation("File upload directories initialized successfully");
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to create upload directories");
}


// *** Initialize Database and Log UPay Configuration ***
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var upayLogger = scope.ServiceProvider.GetRequiredService<ILogger<IUpayService>>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Log UPay configuration (without sensitive data)
        var upayConfig = builder.Configuration.GetSection("UPay");
        upayLogger.LogInformation("UPay Configuration Loaded - Environment: {Environment}, ServiceId: {ServiceId}",
            upayConfig["Environment"], upayConfig["ServiceId"]);

        logger.LogInformation("Application initialized successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to initialize application");
        throw;
    }
}


app.Run();