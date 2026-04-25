using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Ocelot
builder.Services.AddOcelot();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// Rewrite rules for SPA (Single Page Application)
var rewriteOptions = new RewriteOptions();

// Admin app: rewrite requests to /admin/index.html if not a file
rewriteOptions.AddRewrite("^admin(?!/[^/]*\\.[^/]*$)(/.*)?$", "/admin/index.html", skipRemainingRules: false);

// User app: rewrite requests to /user/index.html if not a file
rewriteOptions.AddRewrite("^user(?!/[^/]*\\.[^/]*$)(/.*)?$", "/user/index.html", skipRemainingRules: false);

app.UseRewriter(rewriteOptions);

// Serve static files for admin and user apps
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "admin")),
    RequestPath = "/admin",
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 1 hour
        ctx.Context.Response.Headers.CacheControl = "public,max-age=3600";
    }
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "user")),
    RequestPath = "/user",
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 1 hour
        ctx.Context.Response.Headers.CacheControl = "public,max-age=3600";
    }
});

// Serve root index.html for main app
app.MapGet("/", async context =>
{
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "admin", "index.html");
    if (System.IO.File.Exists(filePath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(filePath);
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Admin app not found. Please build and copy dist/admin to wwwroot/admin");
    }
});

// Ocelot must be last
await app.UseOcelot();

Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════╗
║              BaseCore API Gateway                            ║
║══════════════════════════════════════════════════════════════║
║  Gateway:        http://localhost:5000                       ║
║  Auth Service:   http://localhost:5002                       ║
║  API Service:    http://localhost:5001                      ║
║                                                              ║
║  Admin App:      http://localhost:5000/admin/                ║
║  User App:       http://localhost:5000/user/                 ║
║                                                              ║
║  API Docs:       http://localhost:5000/swagger               ║
╚══════════════════════════════════════════════════════════════╝
");

app.Run();
