﻿using DatingApp.Repository.EntityContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Repository.Repository
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
        public void Insert(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            _context.Add(entity);
            _context.SaveChanges();
        }
        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            T existing = entities.Find(id);
            _context.Remove(existing);
            _context.SaveChanges();
        }
    }
}