using System.Text.Json;

namespace SportAcademy.Application.DTOs.ChatDtos;

public sealed class ChatApiMessage
{
    public string Role { get; init; } = string.Empty;

    public string? Content { get; init; }

    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    public List<ToolCallDto>? ToolCalls { get; init; }

    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    public string? ToolCallId { get; init; }
}
