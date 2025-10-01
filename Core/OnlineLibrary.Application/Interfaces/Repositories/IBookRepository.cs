using OnlineLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Application.Interfaces.Repositories
{
    public interface IBookRepository
    {
     public void Create(Book book);
     public List<Book>  GetAll();
     public void Delete(int id);
     public Book GetById(int id);
    }
}
