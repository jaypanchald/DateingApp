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
        Task AddGroup(Group group);
        Task RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams prams);
        Task<IEnumerable<Message>> GetMessageThread(string userName, string recipientUserName);
        Task<Message> GetMessageThread(int messagId);

    }

    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private readonly DatingContext _context;

        public MessageRepository(DatingContext contex) : base(contex)
        {
            _context = contex;
        }

        public async Task AddGroup(Group group)
        {
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Message
                .Include(i => i.Sender)
                .Include(i => i.Recipient)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(i => i.Connections)
                .FirstOrDefaultAsync(f => f.Name == groupName);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams prams)
        {
            var query = _context.Message
                .OrderByDescending(o => o.MessageSent)
                .AsQueryable();

            query = prams.Container switch
            {
                "Inbox" => query.Where(w => w.Recipient.UserName == prams.Username
                    && !w.RecipientDeleted),
                "Outbox" => query.Where(w => w.Sender.UserName == prams.Username
                    && !w.SenderDeleted),
                _ => query.Where(w => w.Recipient.UserName == prams.Username
                          && !w.RecipientDeleted && w.DateRead == null)
            };

            
           
            return await PagedList<Message>.CreateAsync(query,
                prams.PageNumber, prams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(string currentUsername, string recipientUserName)
        {
            var messages = _context.Message
                .Include(i => i.Sender).ThenInclude(t => t.Photos)
                .Include(i => i.Recipient).ThenInclude(t => t.Photos)
                .Where(w =>
                        (w.Recipient.UserName == currentUsername
                         && w.Sender.UserName == recipientUserName
                         && !w.RecipientDeleted)
                        ||
                        (w.Recipient.UserName == recipientUserName
                         && w.Sender.UserName == currentUsername
                         && !w.SenderDeleted)
                       );
                
            var unreadMessages = messages.Where(m => m.DateRead == null
                && m.Recipient.UserName == currentUsername).ToList();
            
            if (unreadMessages != null && unreadMessages.Any())
            {
                foreach (var unreadMessage in unreadMessages)
                {
                    unreadMessage.DateRead = System.DateTime.UtcNow;
                    unreadMessage.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }

            return await messages.OrderBy(o => o.MessageSent).ToListAsync(); ;
        }


        public async Task<Message> GetMessageThread(int messagId)
        {
            return await _context.Message.Include(i => i.Sender).ThenInclude(t => t.Photos)
                .Include(i => i.Recipient).ThenInclude(t => t.Photos)
                .FirstOrDefaultAsync(f => f.Id == messagId);
        }

        public async Task RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
            await _context.SaveChangesAsync();
        }
    }
}
