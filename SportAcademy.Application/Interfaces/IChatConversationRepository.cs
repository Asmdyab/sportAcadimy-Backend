using SportAcademy.Domain.Entities;

namespace SportAcademy.Application.Interfaces;

public interface IChatConversationRepository : IBaseRepository<ChatConversation, Guid>
{
    Task<List<ChatConversation>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
