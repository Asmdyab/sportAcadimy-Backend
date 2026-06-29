using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;
using SportAcademy.Infrastructure.Persistence.DBContext;


namespace SportAcademy.Infrastructure.Persistence.Repositories
{
    public class ChatConversationRepository : BaseRepository<ChatConversation, Guid>, IChatConversationRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatConversationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ChatConversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ChatConversation>()
                .Include(x => x.Messages.OrderBy(m => m.CreatedAt))
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ChatConversation>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ChatConversation>()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
