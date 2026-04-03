using RepoAgent.Models;
using RepoAgent.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AgentSettings>(builder.Configuration.GetSection("Agent"));
builder.Services.AddSingleton<CodeRepository>();

builder.Services.AddMcpServer(options =>
{
    var settings = builder.Configuration.GetSection("Agent").Get<AgentSettings>() ?? new();
    options.ServerInfo = new() { Name = settings.AgentName, Version = "1.0.0" };
})
.WithHttpTransport()
.WithToolsFromAssembly();

var app = builder.Build();

app.MapMcp();

app.Run();
