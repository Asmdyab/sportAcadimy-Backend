using System.Text.Json.Serialization;

namespace SportAcademy.Application.DTOs.ChatDtos;

public sealed class ToolCallDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; init; } = "function";

    [JsonPropertyName("function")]
    public ToolCallFunctionDto Function { get; init; } = null!;
}

public sealed class ToolCallFunctionDto
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("arguments")]
    public string Arguments { get; init; } = string.Empty;
}

public sealed class ToolResultMessageDto
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = "tool";

    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;

    [JsonPropertyName("tool_call_id")]
    public string ToolCallId { get; init; } = string.Empty;
}
