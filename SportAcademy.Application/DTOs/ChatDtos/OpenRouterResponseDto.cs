using System.Text.Json;

namespace SportAcademy.Application.DTOs.ChatDtos;

public sealed class OpenRouterResponseDto
{
    public List<OpenRouterChoiceDto> Choices { get; init; } = [];
}

public sealed class OpenRouterChoiceDto
{
    public OpenRouterMessageDto Message { get; init; } = null!;

    public string FinishReason { get; init; } = string.Empty;
}

public sealed class OpenRouterMessageDto
{
    public string Role { get; init; } = string.Empty;

    public string? Content { get; init; }

    public List<ToolCallDto>? ToolCalls { get; init; }
}
