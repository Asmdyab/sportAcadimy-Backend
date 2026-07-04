using System.Text.Json;
using SportAcademy.Application.DTOs.ChatDtos;
using SportAcademy.Application.Interfaces;

namespace SportAcademy.Application.Services.ToolPlugins;

public class SportsPlugin : IToolPlugin
{
    private readonly ISportRepository _sportRepository;
    private readonly ISportPriceRepository _sportPriceRepository;

    public string PluginName => "sports";
    public string Description => "Query sports, pricing, and branch availability";

    public SportsPlugin(
        ISportRepository sportRepository,
        ISportPriceRepository sportPriceRepository)
    {
        _sportRepository = sportRepository;
        _sportPriceRepository = sportPriceRepository;
    }

    public IReadOnlyList<ToolDefinitionDto> GetTools()
    {
        return new List<ToolDefinitionDto>
        {
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_all_sports",
                    Description = "Get a list of all sports available at the academy with their details (name, description, category)",
                    Parameters = JsonSerializer.SerializeToElement(new { type = "object", properties = new { } })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_sport_by_id",
                    Description = "Get detailed information about a specific sport by its ID",
                    Parameters = JsonSerializer.SerializeToElement(new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["sportId"] = new { type = "integer", description = "The ID of the sport" }
                        },
                        required = new[] { "sportId" }
                    })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_sports_for_branch",
                    Description = "Get all sports available at a specific branch",
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
                    Name = "get_sport_prices",
                    Description = "Get pricing information for sports. Optionally filter by sport and/or branch.",
                    Parameters = JsonSerializer.SerializeToElement(new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["sportId"] = new { type = "integer", description = "Optional sport ID to filter prices" },
                            ["branchId"] = new { type = "integer", description = "Optional branch ID to filter prices" }
                        }
                    })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "search_sports",
                    Description = "Search for sports by name",
                    Parameters = JsonSerializer.SerializeToElement(new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["term"] = new { type = "string", description = "Search term to find sports by name" }
                        },
                        required = new[] { "term" }
                    })
                }
            }
        };
    }

    public async Task<string> ExecuteAsync(string toolName, string argumentsJson, CancellationToken cancellationToken)
    {
        return toolName switch
        {
            "get_all_sports" => await GetAllSports(cancellationToken),
            "get_sport_by_id" => await GetSportById(argumentsJson, cancellationToken),
            "get_sports_for_branch" => await GetSportsForBranch(argumentsJson, cancellationToken),
            "get_sport_prices" => await GetSportPrices(argumentsJson, cancellationToken),
            "search_sports" => await SearchSports(argumentsJson, cancellationToken),
            _ => JsonSerializer.Serialize(new { error = $"Unknown tool: {toolName}" })
        };
    }

    private async Task<string> GetAllSports(CancellationToken cancellationToken)
    {
        var sports = await _sportRepository.GetAllAsync(cancellationToken);
        var result = sports.Select(s => new
        {
            id = s.Id,
            name = s.Name,
            description = s.Description,
            category = s.Category.ToString(),
            requiresHealthTest = s.IsRequireHealthTest
        });
        return JsonSerializer.Serialize(result);
    }

    private async Task<string> GetSportById(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var sportId = doc.RootElement.GetProperty("sportId").GetInt32();
        var sport = await _sportRepository.GetByIdAsync(sportId, cancellationToken);
        if (sport == null)
            return JsonSerializer.Serialize(new { error = $"Sport with ID {sportId} not found" });

        return JsonSerializer.Serialize(new
        {
            id = sport.Id,
            name = sport.Name,
            description = sport.Description,
            category = sport.Category.ToString(),
            requiresHealthTest = sport.IsRequireHealthTest,
            isDeleted = sport.IsDeleted
        });
    }

    private async Task<string> GetSportsForBranch(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var branchId = doc.RootElement.GetProperty("branchId").GetInt32();
        var sports = await _sportRepository.GetAvailableSportsForBranch(branchId, cancellationToken);
        var result = sports.Select(s => new
        {
            id = s.Id,
            name = s.Name,
            description = s.Description,
            category = s.Category.ToString()
        });
        return JsonSerializer.Serialize(result);
    }

    private async Task<string> GetSportPrices(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var hasSportId = doc.RootElement.TryGetProperty("sportId", out var sportIdProp);
        var hasBranchId = doc.RootElement.TryGetProperty("branchId", out var branchIdProp);

        var prices = await _sportPriceRepository.GetAllWithIncludesAsync(cancellationToken);

        if (hasSportId)
        {
            var sid = sportIdProp.GetInt32();
            prices = prices.Where(p => p.SportId == sid).ToList();
        }
        if (hasBranchId)
        {
            var bid = branchIdProp.GetInt32();
            prices = prices.Where(p => p.BranchId == bid).ToList();
        }

        var result = prices.Select(p => new
        {
            sportId = p.SportId,
            branchId = p.BranchId,
            subscriptionTypeId = p.SubsTypeId,
            subscriptionTypeName = p.SportSubscriptionType?.SubscriptionType?.Name.ToString(),
            price = p.Price
        });
        return JsonSerializer.Serialize(result);
    }

    private async Task<string> SearchSports(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var term = doc.RootElement.GetProperty("term").GetString() ?? "";
        var sports = await _sportRepository.SearchNameAsync(term, cancellationToken);
        return JsonSerializer.Serialize(sports);
    }
}
