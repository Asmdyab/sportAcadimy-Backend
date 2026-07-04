using SportAcademy.Application.DTOs.ChatDtos;

namespace SportAcademy.Application.Interfaces;

public interface IToolRegistry
{
    IReadOnlyList<ToolDefinitionDto> GetAllTools();

    Task<string> ExecuteToolAsync(string toolName, string argumentsJson, CancellationToken cancellationToken);
}
