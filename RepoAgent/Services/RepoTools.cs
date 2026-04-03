using System.ComponentModel;
using ModelContextProtocol.Server;

namespace RepoAgent.Services;

[McpServerToolType]
public class RepoTools(CodeRepository repo)
{
    [McpServerTool, Description("Lists all configured repositories and their paths")]
    public string ListRepositories() => repo.ListRepositories();

    [McpServerTool, Description("Lists all files across repositories. Optionally filter by repository name")]
    public string ListFiles(
        [Description("Optional repository name to filter. Omit to list all repos")] string? repoName = null)
        => repo.ListFiles(repoName);

    [McpServerTool, Description("Returns full contents of all code files. Optionally filter by repository name. Results are tagged with [repoName]")]
    public string GetCodeContext(
        [Description("Optional repository name to filter. Omit to get all repos")] string? repoName = null)
        => repo.GetCodeContext(repoName);

    [McpServerTool, Description("Returns content of a specific file from a named repository")]
    public string GetFile(
        [Description("Repository name, e.g. 'backend'")] string repoName,
        [Description("Relative file path, e.g. 'src/Program.cs'")] string relativePath)
        => repo.GetFile(repoName, relativePath);

    [McpServerTool, Description("Searches for text across repositories. Results are tagged with [repoName]. Optionally filter by repository name")]
    public string SearchCode(
        [Description("Text to search for in file contents")] string query,
        [Description("Optional repository name to filter. Omit to search all repos")] string? repoName = null)
        => repo.Search(query, repoName);
}
