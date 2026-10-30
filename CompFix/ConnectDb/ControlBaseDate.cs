using CompFix_ConnectDb;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace CompFix.ConnectDb
{
    // клас для керування базою даних реалізація CRUD
    public class ControlBaseDate
    {
        public AppDbContext2 item2 = new AppDbContext2(12, "Hello");
        private AppDbContext CreatContext()
        {
            return new AppDbContext();
        }
        public async Task<int> CreateAsync(AppDbContext2 item)// для додавання моделей(ну обєетів)
        {
            await using AppDbContext db = CreatContext();
            db.DataBaseInfo.Add(item);
            await db.SaveChangesAsync();
            Console.WriteLine("Все успішно");

            return item.Id;
        }
        public async Task<List<AppDbContext2>> AllReadAsync()
        {
            await using AppDbContext db = CreatContext();
            return await db.DataBaseInfo.OrderByDescending(item => item.Id).ToListAsync();
        }
        public async Task<bool> UpDateAsync(int id, int number, string sentens)
        {
            await using AppDbContext db = CreatContext();
            AppDbContext2? item = await db.DataBaseInfo.FindAsync(id);
            if (item == null)
            {
                return false;
            }
            item.Number = number;
            item.Sentens = sentens;
            await db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveAsync(int id)
        {
            await using AppDbContext db = CreatContext();
            AppDbContext2? item = await db.DataBaseInfo.FindAsync(id);
            if (item == null)
            {
                return false;
            }
            db.DataBaseInfo.Remove(item);
            await db.SaveChangesAsync();
            return true;

        }

    }
}

