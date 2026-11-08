using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CompFix.Models;
namespace CompFix.Services

// клас для керування базою даних реалізація CRUD
{
    public class ControlBaseDate<T> where T : class
    {
        private readonly AppDbContext db;
        public ControlBaseDate(AppDbContext db)
        {
            this.db = db;
        }
        
        public async Task AddAsync(T item)
        {
            await db.Set<T>().AddAsync(item);
            await db.SaveChangesAsync();
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await db.Set<T>().ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await db.Set<T>().FindAsync(id);
        }
        public async Task<bool> RemoveAsync(int id)
        {
            var item = await db.Set<T>().FindAsync(id);
            if (item == null)
            {
                return false;
            }

            db.Set<T>().Remove(item);
            await db.SaveChangesAsync();
            return true;
        }
        public async Task UpdateAsync(T item)
        {
            db.Set<T>().Update(item);
            await db.SaveChangesAsync();
        }
        public IQueryable<T> GetQueryable()
        {
            return db.Set<T>().AsQueryable();
        }

    }

}


