using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dating.Repository.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> FindOne(int id);
        Task<bool> Insert(T entity);
        Task<bool> Update(T entity);
        Task<bool> UpdateAll(List<T> entity);
        Task<bool> Delete(int id);
        Task<bool> Delete(T entity);
        Task<bool> SaveChangesAsync();
    }
}
