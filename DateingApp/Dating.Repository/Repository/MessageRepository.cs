using Dating.Model.Entity;
using Dating.Model.Helper;
using Dating.Repository.EntityContext;
using Dating.Repository.PagedList;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dating.Repository.Repository
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesFprUser(MessageParams prams);
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
        Task<Message> GetMessageThread(int messagId);

    }

    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private readonly DatingContext _contex;

        public MessageRepository(DatingContext contex) : base(contex)
        {
            _contex = contex;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _contex.Message.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesFprUser(MessageParams prams)
        {
            var messages = _contex.Message.Include(i => i.Sender).ThenInclude(t => t.Photos)
                .Include(i => i.Recipient).ThenInclude(t => t.Photos)
                .AsQueryable();

            switch (prams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(w => w.Recipientid == prams.UserId 
                    && !w.RecipientDeleted);
                    break;
                case "Outbox":
                    messages = messages.Where(w => w.SenderId == prams.UserId 
                        && !w.SenderDeleted);
                    break;
                default:
                    messages = messages.Where(w => w.Recipientid == prams.UserId 
                    && !w.IsRead && !w.RecipientDeleted);
                    break;
            }
            messages = messages.OrderBy(o => o.MessageSent);

            return await PagedList<Message>.CreateAsync(messages,
                prams.PageNumber, prams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _contex.Message.Include(i => i.Sender).ThenInclude(t => t.Photos)
                .Include(i => i.Recipient).ThenInclude(t => t.Photos)
                .Where(w => (w.Recipientid == userId && w.SenderId == recipientId && !w.RecipientDeleted)
                || (w.Recipientid == recipientId && w.SenderId == userId && !w.SenderDeleted))
                .OrderByDescending(o => o.MessageSent)
                .ToListAsync();

            return messages;
        }

        public async Task<Message> GetMessageThread(int messagId)
        {
            return await _contex.Message.Include(i => i.Sender).ThenInclude(t => t.Photos)
                .Include(i => i.Recipient).ThenInclude(t => t.Photos)
                .FirstOrDefaultAsync(f => f.Id == messagId);
        }
    }
}
