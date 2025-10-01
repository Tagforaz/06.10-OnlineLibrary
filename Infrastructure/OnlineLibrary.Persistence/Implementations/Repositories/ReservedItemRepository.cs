using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Application.Interfaces.Repositories;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Persistence.Implementations.Repositories
{
    public class ReservedItemRepository:IReservedItemRepository
    {
        private readonly AppDbContext _context;
        public ReservedItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Create(ReservedItem item)
        {
            _context.ReservedItems.Add(item);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            ReservedItem? item = GetById(id);
            if (item != null)
            {
                _context.ReservedItems.Remove(item);
                _context.SaveChanges();
            }
        }

        public List<ReservedItem> GetAll()
        {
           return _context.ReservedItems.Include(r=>r.Book).ToList();
        }

        public ReservedItem? GetById(int id)
        {
            return _context.ReservedItems.Include(r => r.Book).FirstOrDefault(r => r.Id == id);
        }
    }
}
