using System.Text.Json;
using System.Text.Json.Serialization;

namespace SportAcademy.Application.DTOs.ChatDtos;

public sealed class ToolDefinitionDto
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "function";

    [JsonPropertyName("function")]
    public FunctionDefinitionDto Function { get; init; } = null!;
}

public sealed class FunctionDefinitionDto
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("parameters")]
    public JsonElement Parameters { get; init; }
}
