using OnlineLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Application.Interfaces.Repositories
{
    public interface IAuthorRepository
    {
        public void Create(Author author);
        public List<Author> GetAll();
        public void Delete(int id);
        public Author? GetById(int id);  

    }
}
