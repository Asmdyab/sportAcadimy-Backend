using System.Text.Json;
using SportAcademy.Application.DTOs.ChatDtos;
using SportAcademy.Application.Interfaces;

namespace SportAcademy.Application.Services.ToolPlugins;

public class BranchCoachPlugin : IToolPlugin
{
    private readonly IBranchRepository _branchRepository;
    private readonly ICoachRepository _coachRepository;

    public string PluginName => "branches_coaches";
    public string Description => "Query branch locations and coach information";

    public BranchCoachPlugin(
        IBranchRepository branchRepository,
        ICoachRepository coachRepository)
    {
        _branchRepository = branchRepository;
        _coachRepository = coachRepository;
    }

    public IReadOnlyList<ToolDefinitionDto> GetTools()
    {
        return new List<ToolDefinitionDto>
        {
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_all_branches",
                    Description = "Get a list of all academy branches with their location details",
                    Parameters = JsonSerializer.SerializeToElement(new { type = "object", properties = new { } })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_branch_stats",
                    Description = "Get statistics for a specific branch including total coaches, trainees, and groups",
                    Parameters = JsonSerializer.SerializeToElement(new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["branchId"] = new { type = "integer", description = "The ID of the branch" }
                        },
                        required = new[] { "branchId" }
                    })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_coaches",
                    Description = "Get a list of all coaches optionally filtered by sport ID",
                    Parameters = JsonSerializer.SerializeToElement(new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["sportId"] = new { type = "integer", description = "Optional sport ID to filter coaches" }
                        }
                    })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_coach_by_id",
                    Description = "Get detailed information about a specific coach including their sport and branch",
                    Parameters = JsonSerializer.SerializeToElement(new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["coachId"] = new { type = "integer", description = "The ID of the coach" }
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
            "get_all_branches" => await GetAllBranches(cancellationToken),
            "get_branch_stats" => await GetBranchStats(argumentsJson, cancellationToken),
            "get_coaches" => await GetCoaches(cancellationToken),
            "get_coach_by_id" => await GetCoachById(argumentsJson, cancellationToken),
            _ => JsonSerializer.Serialize(new { error = $"Unknown tool: {toolName}" })
        };
    }

    private async Task<string> GetAllBranches(CancellationToken cancellationToken)
    {
        var branches = await _branchRepository.GetAllBranchsBase(cancellationToken);
        return JsonSerializer.Serialize(branches);
    }

    private async Task<string> GetBranchStats(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var branchId = doc.RootElement.GetProperty("branchId").GetInt32();
        var stats = await _branchRepository.GetBranchStatsAsync(branchId, cancellationToken);
        return JsonSerializer.Serialize(stats);
    }

    private async Task<string> GetCoaches(CancellationToken cancellationToken)
    {
        var coaches = await _coachRepository.GetDropdownListAsync(cancellationToken);
        return JsonSerializer.Serialize(coaches);
    }

    private async Task<string> GetCoachById(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var coachId = doc.RootElement.GetProperty("coachId").GetInt32();
        var coach = await _coachRepository.GetByIdWithDetailsAsync(coachId, cancellationToken);
        if (coach == null)
            return JsonSerializer.Serialize(new { error = $"Coach with ID {coachId} not found" });

        return JsonSerializer.Serialize(new
        {
            id = coach.EmployeeId,
            firstName = coach.Employee?.FirstName,
            lastName = coach.Employee?.LastName,
            sportId = coach.SportId,
            sportName = coach.Sport?.Name,
            skillLevel = coach.SkillLevel.ToString(),
            rate = coach.Rate
        });
    }
}
