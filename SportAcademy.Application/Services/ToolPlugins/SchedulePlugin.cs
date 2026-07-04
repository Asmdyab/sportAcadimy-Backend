using System.Text.Json;
using SportAcademy.Application.DTOs.ChatDtos;
using SportAcademy.Application.Interfaces;

namespace SportAcademy.Application.Services.ToolPlugins;

public class SchedulePlugin : IToolPlugin
{
    private readonly ITraineeGroupRepository _traineeGroupRepository;
    private readonly ISessionOccurrenceRepository _sessionOccurrenceRepository;

    public string PluginName => "schedules";
    public string Description => "Query training group schedules and session occurrences";

    public SchedulePlugin(
        ITraineeGroupRepository traineeGroupRepository,
        ISessionOccurrenceRepository sessionOccurrenceRepository)
    {
        _traineeGroupRepository = traineeGroupRepository;
        _sessionOccurrenceRepository = sessionOccurrenceRepository;
    }

    public IReadOnlyList<ToolDefinitionDto> GetTools()
    {
        return new List<ToolDefinitionDto>
        {
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_trainee_groups",
                    Description = "Get a list of all training groups with their details (branch, coach, skill level, capacity)",
                    Parameters = JsonSerializer.SerializeToElement(new { type = "object", properties = new { } })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_group_details",
                    Description = "Get detailed information about a specific training group including schedules",
                    Parameters = JsonSerializer.SerializeToElement(new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["groupId"] = new { type = "integer", description = "The ID of the training group" }
                        },
                        required = new[] { "groupId" }
                    })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_sessions_by_date",
                    Description = "Get all training sessions scheduled for a specific date. Optionally filter by group.",
                    Parameters = JsonSerializer.SerializeToElement(new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["date"] = new { type = "string", description = "The date in yyyy-MM-dd format" },
                            ["groupId"] = new { type = "integer", description = "Optional group ID to filter sessions" }
                        },
                        required = new[] { "date" }
                    })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_groups_for_coach",
                    Description = "Get all training groups assigned to a specific coach",
                    Parameters = JsonSerializer.SerializeToElement(new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["coachId"] = new { type = "integer", description = "The coach's employee ID" }
                        },
                        required = new[] { "coachId" }
                    })
                }
            }
        };
    }

    public async Task<string> ExecuteAsync(string toolName, string argumentsJson, CancellationToken cancellationToken)
    {
        return toolName switch
        {
            "get_trainee_groups" => await GetAllGroups(cancellationToken),
            "get_group_details" => await GetGroupDetails(argumentsJson, cancellationToken),
            "get_sessions_by_date" => await GetSessionsByDate(argumentsJson, cancellationToken),
            "get_groups_for_coach" => await GetGroupsForCoach(argumentsJson, cancellationToken),
            _ => JsonSerializer.Serialize(new { error = $"Unknown tool: {toolName}" })
        };
    }

    private async Task<string> GetAllGroups(CancellationToken cancellationToken)
    {
        var groups = await _traineeGroupRepository.GetDropdownListAsync(cancellationToken);
        return JsonSerializer.Serialize(groups);
    }

    private async Task<string> GetGroupDetails(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var groupId = doc.RootElement.GetProperty("groupId").GetInt32();
        var group = await _traineeGroupRepository.GetDetailsByIdAsync(groupId, cancellationToken);
        if (group == null)
            return JsonSerializer.Serialize(new { error = $"Group with ID {groupId} not found" });

        return JsonSerializer.Serialize(group);
    }

    private async Task<string> GetSessionsByDate(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var date = DateTime.Parse(doc.RootElement.GetProperty("date").GetString()!);
        var hasGroupId = doc.RootElement.TryGetProperty("groupId", out var groupIdProp);
        int? groupId = hasGroupId ? groupIdProp.GetInt32() : null;

        var queryParams = Application.Common.Pagination.PageRequest.Create(1, 50);
        var sessions = await _sessionOccurrenceRepository.GetByDateAsync(
            date, queryParams, groupId, cancellationToken);
        return JsonSerializer.Serialize(sessions);
    }

    private async Task<string> GetGroupsForCoach(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var coachId = doc.RootElement.GetProperty("coachId").GetInt32();
        var groups = await _traineeGroupRepository.GetDropdownListAsync(cancellationToken);
        var filtered = groups.Where(g =>
        {
            var prop = g.GetType().GetProperty("CoachId") ?? g.GetType().GetProperty("coachId");
            if (prop != null && prop.GetValue(g) is int cid)
                return cid == coachId;
            return false;
        });
        return JsonSerializer.Serialize(filtered);
    }
}
