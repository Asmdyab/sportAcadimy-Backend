using SportAcademy.Application.DTOs.ChatDtos;

namespace SportAcademy.Application.Interfaces;

public interface IToolPlugin
{
    string PluginName { get; }

    string Description { get; }

    IReadOnlyList<ToolDefinitionDto> GetTools();

    Task<string> ExecuteAsync(string toolName, string argumentsJson, CancellationToken cancellationToken);
}
