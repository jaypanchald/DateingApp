using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Repository.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> FindOne(int id);
        Task<bool> Insert(T entity);
        Task<bool> Update(T entity);
        void Delete(int id);
    }
}
