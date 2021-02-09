using Dating.Model.Entity;
using DatingApp.Repository.EntityContext;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DatingApp.Repository.Repository
{
    public interface ILikeRepository : IRepository<Like>
    {
        Task<Like> GetLike(int userId, int recipientId);
    }

    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        private readonly DatingContext _contex;
        public LikeRepository(DatingContext contex) : base(contex)
        {
            _contex = contex;
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _contex.Like.FirstOrDefaultAsync(f =>
            f.LikerId == userId
           && f.LikeeId == recipientId);
        }
    }
}
