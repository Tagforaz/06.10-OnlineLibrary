using OnlineLibrary.Application.Interfaces.Repositories;
using OnlineLibrary.Application.Interfaces.Services;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Persistence.Implementations.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _books;
        private readonly IAuthorRepository _authors;
        private readonly IReservedItemRepository _reservedItem;
        public BookService(IBookRepository books, IAuthorRepository authors, IReservedItemRepository reservedItem)
        {
            _books = books;
            _authors = authors;
            _reservedItem = reservedItem;
        }

        public void Create(string name, int pageCount, int authorId)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("Book name is also used");
            var author = _authors.GetById(authorId);
            if (author == null) throw new ArgumentNullException("Author not founded");
            var book = new Book
            {
                Name = name.Trim(),
                PageCount = pageCount,
                AuthorId = authorId,
                Author = author
            };
            _books.Create(book);
        }

        public void Delete(int id)
        {
          var book = _books.GetById(id);
            if(book == null) return;
            var hasActive = book.ReservedItems.Any(r => r.Status == Status.Confirmed || r.Status == Status.Started);
            if (hasActive) throw new ArgumentException("Book is reserved");
            _books.Delete(id);
        }

        public List<Book> GetAll()
        {
            return _books.GetAll();
        }

      
        public Book? GetById(int id, bool withDate = false)
        {
            var book = _books.GetById(id);
            if(book == null) return null;
            if (!withDate)
            {
                book.ReservedItems = new List<ReservedItem>() ;
            }
            return book;
        }

        public List<ReservedItem> GetBookReservInfo(int bookId)
        {
            var book = _books.GetById(bookId);
            return book?.ReservedItems.OrderByDescending(r => r.StartDate).ToList()
                   ?? new List<ReservedItem>();
        }

       
    }
}
