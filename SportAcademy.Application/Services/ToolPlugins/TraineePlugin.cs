using System.Text.Json;
using SportAcademy.Application.DTOs.ChatDtos;
using SportAcademy.Application.Interfaces;

namespace SportAcademy.Application.Services.ToolPlugins;

public class TraineePlugin : IToolPlugin
{
    private readonly ITraineeRepository _traineeRepository;

    public string PluginName => "trainees";
    public string Description => "Query trainee information and enrollment details";

    public TraineePlugin(ITraineeRepository traineeRepository)
    {
        _traineeRepository = traineeRepository;
    }

    public IReadOnlyList<ToolDefinitionDto> GetTools()
    {
        return new List<ToolDefinitionDto>
        {
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_trainee_by_id",
                    Description = "Get detailed information about a trainee by their ID. Returns personal info, sports, and subscription status.",
                    Parameters = JsonSerializer.SerializeToElement(new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["traineeId"] = new { type = "integer", description = "The ID of the trainee" }
                        },
                        required = new[] { "traineeId" }
                    })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "search_trainees",
                    Description = "Search for trainees by name or other criteria",
                    Parameters = JsonSerializer.SerializeToElement(new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["term"] = new { type = "string", description = "Search term to find trainees" }
                        },
                        required = new[] { "term" }
                    })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_trainee_count",
                    Description = "Get the total number of trainees registered at the academy",
                    Parameters = JsonSerializer.SerializeToElement(new { type = "object", properties = new { } })
                }
            }
        };
    }

    public async Task<string> ExecuteAsync(string toolName, string argumentsJson, CancellationToken cancellationToken)
    {
        return toolName switch
        {
            "get_trainee_by_id" => await GetTraineeById(argumentsJson, cancellationToken),
            "search_trainees" => await SearchTrainees(argumentsJson, cancellationToken),
            "get_trainee_count" => await GetTraineeCount(cancellationToken),
            _ => JsonSerializer.Serialize(new { error = $"Unknown tool: {toolName}" })
        };
    }

    private async Task<string> GetTraineeById(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var traineeId = doc.RootElement.GetProperty("traineeId").GetInt32();
        var trainee = await _traineeRepository.GetFullTrainee(traineeId, cancellationToken);
        if (trainee == null)
            return JsonSerializer.Serialize(new { error = $"Trainee with ID {traineeId} not found" });

        return JsonSerializer.Serialize(new
        {
            id = trainee.Id,
            firstName = trainee.FirstName,
            lastName = trainee.LastName,
            birthDate = trainee.BirthDate.ToString(),
            gender = trainee.Gender.ToString(),
            isSubscribed = trainee.IsSubscribed,
            joinDate = trainee.JoinDate.ToString(),
            age = trainee.GetAge(),
            ageCategory = trainee.AgeCategory.ToString(),
            sports = trainee.Sports?.Select(s => new
            {
                sportId = s.SportId,
                sportName = s.Sport?.Name,
                skillLevel = s.SkillLevel.ToString()
            }),
            branchId = trainee.BranchId
        });
    }

    private async Task<string> SearchTrainees(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var term = doc.RootElement.GetProperty("term").GetString() ?? "";
        var trainees = await _traineeRepository.GetDropdownAsync(cancellationToken);
        var filtered = trainees.Where(t =>
            $"{t.FirstName} {t.LastName}".Contains(term, StringComparison.OrdinalIgnoreCase) ||
            t.Id.ToString() == term);
        return JsonSerializer.Serialize(filtered);
    }

    private async Task<string> GetTraineeCount(CancellationToken cancellationToken)
    {
        var count = await _traineeRepository.CountAsync(cancellationToken);
        return JsonSerializer.Serialize(new { totalTrainees = count });
    }
}
