using OnlineLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Application.Interfaces.Services
{
    public interface IBookService
    {
        void Create(Book book);
        void Delete(int id);
        Book? GetById(int id, bool withDate = false);
        List<Book> GetAll();
        List<ReservedItem> GetBookReservInfo(int bookId);
    }
}
