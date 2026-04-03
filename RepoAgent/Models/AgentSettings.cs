namespace RepoAgent.Models;

public class AgentSettings
{
    public string AgentName { get; set; } = "RepoAgent";
    public Dictionary<string, string> Repositories { get; set; } = [];
    public string[] FileExtensions { get; set; } = [".cs", ".csproj", ".json", ".xml", ".md", ".yaml", ".yml", ".txt", ".sln", ".tf", ".ts", ".js"];
    public int MaxFileSizeKb { get; set; } = 100;
}
