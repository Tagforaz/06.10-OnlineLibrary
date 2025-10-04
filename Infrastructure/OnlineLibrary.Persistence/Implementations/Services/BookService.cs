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
        public BookService(IBookRepository books, IAuthorRepository authors)
        {
            _books = books;
            _authors = authors;
        }

        public void Create(Book book)
        {
            if (book is null) throw new ArgumentNullException(nameof(book), "Book's name wasn't be null");
            if (string.IsNullOrWhiteSpace(book.Name)) throw new ArgumentException("Book's name was used",nameof(book.Name));
            if (book.PageCount <= 0) throw new ArgumentException("Page's count cant be zero or negative", nameof(book.PageCount));
            var author =_authors.GetById(book.AuthorId);
            if (author is null) throw new InvalidOperationException("Author not founded");
            book.Name = book.Name.Trim();
            book.Author= author;
            _books.Create(book);
        }

        public void Delete(int id)
        {
          var book = _books.GetById(id);
            if(book is null) throw new InvalidOperationException("Book not found.");
            var hasActive = book.ReservedItems.Any(r => r.Status == Status.Confirmed || r.Status == Status.Started);
            if (hasActive) throw new InvalidOperationException("Book is reserved");
            _books.Delete(id);
        }

        public List<Book> GetAll()
        {
            return _books.GetAll();
        }

      
        public Book? GetById(int id, bool withDate = false)
        {
            var book = _books.GetById(id);
            if(book is null) return null;
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
