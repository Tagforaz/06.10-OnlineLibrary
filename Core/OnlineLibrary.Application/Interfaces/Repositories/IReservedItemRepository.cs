using OnlineLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Application.Interfaces.Repositories
{
    public interface IReservedItemRepository
    {
        public void Create(ReservedItem item);
        public void Delete(int id);
        public List<ReservedItem> GetAll();
        public ReservedItem GetById(int id);

    }
}
