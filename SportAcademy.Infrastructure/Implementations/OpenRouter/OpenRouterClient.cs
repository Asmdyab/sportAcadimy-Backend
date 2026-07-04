using Microsoft.Extensions.Configuration;
using SportAcademy.Application.DTOs.ChatDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SportAcademy.Infrastructure.Implementations.OpenRouter;

public class OpenRouterClient : IOpenRouterClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public OpenRouterClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenRouterSettings:ApiKey"]!;
        _model = configuration["OpenRouterSettings:Model"] ?? "mistralai/mistral-7b-instruct:free";
    }

    public async Task<string> SendAsync(
        string systemPrompt,
        string userMessage,
        CancellationToken cancellationToken)
    {
        var request = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post,
            "https://openrouter.ai/api/v1/chat/completions");

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        httpRequest.Headers.Add("HTTP-Referer", "https://sportacademy.app");
        httpRequest.Headers.Add("X-Title", "SportAcademy Video Analysis");

        httpRequest.Content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"OpenRouter API error ({response.StatusCode}): {json}");
        }

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()!;
    }

    public async Task<string> SendMessagesAsync(
        IReadOnlyList<OpenAiMessage> messages,
        CancellationToken cancellationToken)
    {
        var request = new
        {
            model = _model,
            messages = messages.Select(m => new { role = m.Role.ToString().ToLowerInvariant(), content = m.Content })
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post,
            "https://openrouter.ai/api/v1/chat/completions");

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        httpRequest.Headers.Add("HTTP-Referer", "https://sportacademy.app");
        httpRequest.Headers.Add("X-Title", "SportAcademy Chatbot");

        httpRequest.Content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"OpenRouter API error ({response.StatusCode}): {json}");
        }

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()!;
    }

    public async Task<OpenRouterResponseDto> SendWithToolsAsync(
        IReadOnlyList<ChatApiMessage> messages,
        IReadOnlyList<ToolDefinitionDto>? tools,
        CancellationToken cancellationToken)
    {
        var requestObj = new Dictionary<string, object>
        {
            ["model"] = _model,
            ["messages"] = messages.Select(BuildMessagePayload).ToList()
        };

        if (tools is { Count: > 0 })
        {
            requestObj["tools"] = tools;
        }

        var httpRequest = new HttpRequestMessage(HttpMethod.Post,
            "https://openrouter.ai/api/v1/chat/completions");

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        httpRequest.Headers.Add("HTTP-Referer", "https://sportacademy.app");
        httpRequest.Headers.Add("X-Title", "SportAcademy Chatbot");

        httpRequest.Content = new StringContent(
            JsonSerializer.Serialize(requestObj),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"OpenRouter API error ({response.StatusCode}): {json}");
        }

        var result = JsonSerializer.Deserialize<OpenRouterResponseDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result!;
    }

    private static object BuildMessagePayload(ChatApiMessage msg)
    {
        if (msg.Role == "tool")
        {
            return new
            {
                role = "tool",
                tool_call_id = msg.ToolCallId,
                content = msg.Content ?? ""
            };
        }

        if (msg.ToolCalls is { Count: > 0 })
        {
            return new
            {
                role = "assistant",
                content = msg.Content,
                tool_calls = msg.ToolCalls.Select(tc => new
                {
                    id = tc.Id,
                    type = "function",
                    function = new
                    {
                        name = tc.Function.Name,
                        arguments = tc.Function.Arguments
                    }
                })
            };
        }

        return new
        {
            role = msg.Role,
            content = msg.Content ?? ""
        };
    }
}
