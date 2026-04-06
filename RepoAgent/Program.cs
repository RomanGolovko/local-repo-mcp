using RepoAgent.Models;
using RepoAgent.Services;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Logging.ClearProviders();
builder.Logging.AddDebug();

builder.Services.Configure<AgentSettings>(builder.Configuration.GetSection("Agent"));
builder.Services.AddSingleton<CodeRepository>();

builder.Services.AddMcpServer(options =>
{
    var settings = builder.Configuration.GetSection("Agent").Get<AgentSettings>() ?? new();
    options.ServerInfo = new() { Name = settings.AgentName, Version = "1.0.0" };
})
.WithStdioServerTransport()
.WithToolsFromAssembly();

var app = builder.Build();

await app.RunAsync();
