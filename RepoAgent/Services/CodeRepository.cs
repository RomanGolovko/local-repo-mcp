using System.Text;
using Microsoft.Extensions.Options;
using RepoAgent.Models;

namespace RepoAgent.Services;

public class CodeRepository(IOptions<AgentSettings> settings, ILogger<CodeRepository> logger)
{
    private readonly AgentSettings _settings = settings.Value;

    public string GetCodeContext(string? repoName = null)
    {
        var repos = ResolveRepos(repoName);
        if (repos is []) return "[No repositories configured or found]";

        var sb = new StringBuilder();
        foreach (var (name, path) in repos)
        {
            sb.AppendLine($"=== Repository: {name} ({path}) ===");
            foreach (var file in GetFiles(path))
            {
                var relative = Path.GetRelativePath(path, file);
                try
                {
                    sb.AppendLine($"--- [{name}] {relative} ---");
                    sb.Append(File.ReadAllText(file));
                    sb.AppendLine();
                }
                catch (Exception ex) { logger.LogWarning(ex, "Failed to read {File}", file); }
            }
        }

        return sb.ToString();
    }

    public string GetFile(string repoName, string relativePath)
    {
        if (!_settings.Repositories.TryGetValue(repoName, out var path))
            return $"[Repository '{repoName}' not found. Available: {string.Join(", ", _settings.Repositories.Keys)}]";

        var fullPath = Path.GetFullPath(Path.Combine(path, relativePath));
        if (!fullPath.StartsWith(path, StringComparison.OrdinalIgnoreCase))
            return "[Error: path traversal not allowed]";

        return File.Exists(fullPath) ? $"[{repoName}] {relativePath}:\n{File.ReadAllText(fullPath)}" : $"[File not found: {relativePath} in {repoName}]";
    }

    public string ListFiles(string? repoName = null)
    {
        var repos = ResolveRepos(repoName);
        if (repos is []) return "[No repositories configured or found]";

        var sb = new StringBuilder();
        foreach (var (name, path) in repos)
        {
            sb.AppendLine($"=== {name} ===");
            foreach (var file in GetFiles(path))
                sb.AppendLine($"  {Path.GetRelativePath(path, file)}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public string Search(string query, string? repoName = null)
    {
        var repos = ResolveRepos(repoName);
        var sb = new StringBuilder();

        foreach (var (name, path) in repos)
        {
            foreach (var file in GetFiles(path))
            {
                try
                {
                    var lines = File.ReadAllLines(file);
                    var matches = lines
                        .Index()
                        .Where(x => x.Item.Contains(query, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (matches is []) continue;

                    var relative = Path.GetRelativePath(path, file);
                    sb.AppendLine($"--- [{name}] {relative} ---");
                    foreach (var (i, line) in matches)
                        sb.AppendLine($"  L{i + 1}: {line.TrimStart()}");
                    sb.AppendLine();
                }
                catch { /* skip unreadable */ }
            }
        }

        return sb.Length > 0 ? sb.ToString() : $"[No matches for '{query}']";
    }

    public string ListRepositories()
        => string.Join('\n', _settings.Repositories.Select(r => $"{r.Key}: {r.Value}"));

    private List<KeyValuePair<string, string>> ResolveRepos(string? repoName) =>
        repoName is not null
            ? [.. _settings.Repositories.Where(r => r.Key.Equals(repoName, StringComparison.OrdinalIgnoreCase))]
            : [.. _settings.Repositories];

    private List<string> GetFiles(string path) =>
        !Directory.Exists(path) ? [] :
        [.. _settings.FileExtensions
            .SelectMany(ext => Directory.EnumerateFiles(path, $"*{ext}", SearchOption.AllDirectories))
            .Where(f => new FileInfo(f).Length <= _settings.MaxFileSizeKb * 1024)];
}
