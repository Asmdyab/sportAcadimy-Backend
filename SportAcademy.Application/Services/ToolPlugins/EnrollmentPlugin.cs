using System.Text.Json;
using SportAcademy.Application.DTOs.ChatDtos;
using SportAcademy.Application.Interfaces;

namespace SportAcademy.Application.Services.ToolPlugins;

public class EnrollmentPlugin : IToolPlugin
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ISubscriptionDetailsRepository _subscriptionDetailsRepository;

    public string PluginName => "enrollments";
    public string Description => "Query enrollment and subscription information";

    public EnrollmentPlugin(
        IEnrollmentRepository enrollmentRepository,
        ISubscriptionDetailsRepository subscriptionDetailsRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _subscriptionDetailsRepository = subscriptionDetailsRepository;
    }

    public IReadOnlyList<ToolDefinitionDto> GetTools()
    {
        return new List<ToolDefinitionDto>
        {
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_enrollment_stats",
                    Description = "Get overall enrollment statistics (total, active, pending payment counts)",
                    Parameters = JsonSerializer.SerializeToElement(new { type = "object", properties = new { } })
                }
            },
            new()
            {
                Function = new FunctionDefinitionDto
                {
                    Name = "get_enrollments_for_sport",
                    Description = "Get enrollment count and details for a specific sport",
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
                    Name = "get_trainee_subscriptions",
                    Description = "Get all subscription details for a specific trainee",
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
            }
        };
    }

    public async Task<string> ExecuteAsync(string toolName, string argumentsJson, CancellationToken cancellationToken)
    {
        return toolName switch
        {
            "get_enrollment_stats" => await GetEnrollmentStats(cancellationToken),
            "get_enrollments_for_sport" => await GetEnrollmentsForSport(argumentsJson, cancellationToken),
            "get_trainee_subscriptions" => await GetTraineeSubscriptions(argumentsJson, cancellationToken),
            _ => JsonSerializer.Serialize(new { error = $"Unknown tool: {toolName}" })
        };
    }

    private async Task<string> GetEnrollmentStats(CancellationToken cancellationToken)
    {
        var total = await _enrollmentRepository.GetCountAsync(cancellationToken);
        var active = await _enrollmentRepository.GetActiveCountAsync(cancellationToken);
        var pendingPayment = await _enrollmentRepository.GetPendingPaymentCountAsync(cancellationToken);

        return JsonSerializer.Serialize(new
        {
            totalEnrollments = total,
            activeEnrollments = active,
            pendingPaymentEnrollments = pendingPayment
        });
    }

    private async Task<string> GetEnrollmentsForSport(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var sportId = doc.RootElement.GetProperty("sportId").GetInt32();

        var queryParams = Application.Common.Pagination.PageRequest.Create(1, 10);
        var enrollments = await _enrollmentRepository.GetAllEnrollmentsForSport(
            queryParams, null, null, sportId, cancellationToken);
        return JsonSerializer.Serialize(enrollments);
    }

    private async Task<string> GetTraineeSubscriptions(string argumentsJson, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var traineeId = doc.RootElement.GetProperty("traineeId").GetInt32();

        var subscriptions = await _subscriptionDetailsRepository
            .GetSubscriptionDetailsForTraineeAsync(traineeId, cancellationToken);

        if (subscriptions == null || subscriptions.Count == 0)
            return JsonSerializer.Serialize(new { message = "No subscriptions found for this trainee" });

        var result = subscriptions.Select(s => new
        {
            id = s.Id,
            startDate = s.StartDate.ToString(),
            endDate = s.EndDate.ToString(),
            isActive = s.IsActive,
            sportId = s.SportId,
            branchId = s.BranchId,
            paymentNumber = s.PaymentNumber
        });
        return JsonSerializer.Serialize(result);
    }
}
