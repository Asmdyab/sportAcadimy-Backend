using SportAcademy.Application.DTOs.ChatDtos;
using SportAcademy.Application.Interfaces;

namespace SportAcademy.Application.Services.ToolPlugins;

public class ToolRegistry : IToolRegistry
{
    private readonly Dictionary<string, IToolPlugin> _plugins;
    private readonly Dictionary<string, (IToolPlugin Plugin, ToolDefinitionDto Tool)> _toolMap;

    public ToolRegistry(IEnumerable<IToolPlugin> plugins)
    {
        _plugins = plugins.ToDictionary(p => p.PluginName, p => p);
        _toolMap = [];

        foreach (var plugin in _plugins.Values)
        {
            foreach (var tool in plugin.GetTools())
            {
                _toolMap[tool.Function.Name] = (plugin, tool);
            }
        }
    }

    public IReadOnlyList<ToolDefinitionDto> GetAllTools()
    {
        return _toolMap.Values.Select(x => x.Tool).ToList();
    }

    public async Task<string> ExecuteToolAsync(string toolName, string argumentsJson, CancellationToken cancellationToken)
    {
        if (!_toolMap.TryGetValue(toolName, out var entry))
        {
            return $"{{\"error\": \"Unknown tool: {toolName}\"}}";
        }

        return await entry.Plugin.ExecuteAsync(toolName, argumentsJson, cancellationToken);
    }
}
