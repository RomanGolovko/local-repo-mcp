# RepoAgent

MCP server that exposes multiple local code repositories as tools for LLM-powered assistants like Amazon Q Developer.

Search results and file contents are tagged with repository names so the assistant knows which repo each result comes from.

## Prerequisites

- .NET 10 SDK

## Configuration

Edit `appsettings.json`:

```json
{
  "Agent": {
    "AgentName": "RepoAgent",
    "FileExtensions": [".cs", ".csproj", ".json", ".xml", ".md"],
    "MaxFileSizeKb": 100,
    "Repositories": {
      "backend": "C:\\projects\\backend-api",
      "frontend": "C:\\projects\\frontend-app",
      "shared-lib": "C:\\projects\\shared-library"
    }
  }
}
```

| Setting | Description |
|---|---|
| `AgentName` | MCP server name shown to clients |
| `FileExtensions` | Default file types (can be overridden per repo) |
| `MaxFileSizeKb` | Skip files larger than this |
| `Repositories` | Named repositories — key is the display name, value is the path |

## Build

```bash
cd RepoAgent
dotnet publish -c Release -o bin/publish
```

This produces a standalone executable at `bin/publish/RepoAgent.exe`.

## MCP Tools

| Tool | Parameters | Description |
|---|---|---|
| `ListRepositories` | — | Lists all configured repos and their paths |
| `ListFiles` | `repoName?` | Lists files, optionally filtered by repo |
| `GetCodeContext` | `repoName?` | Returns full file contents, tagged with `[repoName]` |
| `GetFile` | `repoName`, `relativePath` | Returns a specific file from a named repo |
| `SearchCode` | `query`, `repoName?` | Searches text across repos, results tagged with `[repoName]` |

All cross-repo tools tag results with the repository name:

```
--- [backend] Controllers/UserController.cs ---
  L15: public async Task<IActionResult> GetUser(int id)

--- [shared-lib] Models/User.cs ---
  L3: public class User
```

## Connect to Amazon Q Developer

### 1. Add MCP config

Create or edit `~/.aws/amazonq/mcp.json`:

```json
{
  "mcpServers": {
    "repo-agent": {
      "command": "C:\\path\\to\\RepoAgent\\bin\\publish\\RepoAgent.exe",
      "args": []
    }
  }
}
```

> **Note:** Point directly at the published exe. Using `dotnet run` can cause MCP protocol errors because build/restore output pollutes stdout.

### 2. Use in Amazon Q chat

- "List all repositories"
- "Search all repos for authentication logic"
- "Get the code context from the backend repo"
- "Find where User model is defined across all repos"
- "Show me src/Program.cs from the backend repo"
