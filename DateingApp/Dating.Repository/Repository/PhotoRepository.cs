using Dating.Model.Entity;
using Dating.Repository.EntityContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dating.Repository.Repository
{

    public interface IPhotoRepository : IRepository<Photo>
    {
        Task<Photo> GetbyId(int id);
        Task<bool> ExistAnyPhoto(int userId, int id);
        Task<List<Photo>> GetListOfPhotosOfUser(int userId);
    }

    public class PhotoRepository : Repository<Photo>, IPhotoRepository
    {
        private readonly DatingContext _contex;
        public PhotoRepository(DatingContext contex) : base(contex)
        {
            _contex = contex;
        }


        public async Task<Photo> GetbyId(int id)
        {
            return await _contex.Photo.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<bool> ExistAnyPhoto(int userId, int id)
        {
            return await _contex.Photo.AnyAsync(f => f.Id == id && f.UserId == userId);
        }

        public async Task<List<Photo>> GetListOfPhotosOfUser(int userId)
        {
            return await _contex.Photo.Where(f => f.UserId == userId).ToListAsync();
        }
    }
}
