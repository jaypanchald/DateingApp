using Dating.Repository.EntityContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dating.Repository.Repository
{


    public class Repository<T> : IRepository<T> where T : class
    {
        private DbSet<T> entities = null;
        private readonly DatingContext _context;

        public Repository(DatingContext context)
        {
            _context = context;
            entities = _context.Set<T>();

        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await entities.ToListAsync();
        }
        public async Task<T> FindOne(int id)
        {
            return await entities.FindAsync(id);
        }
        public async Task<bool> Insert(T entity)
        {
            
            if (entity == null) throw new ArgumentNullException("entity");
            try
            {
                await _context.AddAsync(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            { throw ex; }
            //return false;
        }
        public async Task<bool> Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAll(List<T> entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(int id)
        {
            T existing = entities.Find(id);
            _context.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(T entity)
        {
            _context.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
