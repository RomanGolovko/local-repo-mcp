# RepoAgent

MCP server that exposes multiple local code repositories as tools for LLM-powered assistants like Amazon Q Developer.

Search results and file contents are tagged with repository names so the assistant knows which repo each result comes from.

## Prerequisites

- .NET 8 SDK

## Configuration

Edit `appsettings.json`:

```json
{
  "Urls": "http://localhost:5000",
  "Agent": {
    "AgentName": "RepoAgent",
    "FileExtensions": [".cs", ".csproj", ".json", ".xml", ".md"],
    "MaxFileSizeKb": 100,
    "Repositories": {
      "backend": {
        "Path": "C:\\projects\\backend-api"
      },
      "frontend": {
        "Path": "C:\\projects\\frontend-app",
        "FileExtensions": [".ts", ".tsx", ".json", ".css", ".html"]
      },
      "shared-lib": {
        "Path": "C:\\projects\\shared-library"
      }
    }
  }
}
```

| Setting | Description |
|---|---|
| `AgentName` | MCP server name shown to clients |
| `FileExtensions` | Default file types (can be overridden per repo) |
| `MaxFileSizeKb` | Skip files larger than this |
| `Repositories` | Named repositories — key is the name, value has `Path` and optional `FileExtensions` |

## Run

```bash
cd RepoAgent
dotnet run
```

Server starts at `http://localhost:5000`.

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

### 1. Start the server

```bash
cd RepoAgent
dotnet run
```

### 2. Add MCP config

Create or edit `~/.aws/amazonq/mcp.json`:

```json
{
  "mcpServers": {
    "repo-agent": {
      "url": "http://localhost:5000"
    }
  }
}
```

### 3. Use in Amazon Q chat

- "List all repositories"
- "Search all repos for authentication logic"
- "Get the code context from the backend repo"
- "Find where User model is defined across all repos"
- "Show me src/Program.cs from the backend repo"
