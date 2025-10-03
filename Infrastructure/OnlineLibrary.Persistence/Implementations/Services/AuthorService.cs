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
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authors;
        public AuthorService(IAuthorRepository authors)
        {
            _authors = authors;
        }
        public void Create(Author author)
        {
            if (author is null) throw new ArgumentNullException(nameof(author), "Author's name wasn't be null");
            if (string.IsNullOrWhiteSpace(author.Name)) throw new ArgumentException("Author name was used",nameof(author.Name));
            author.Name = author.Name.Trim();
            if (!string.IsNullOrWhiteSpace(author.Surname))
            {
                author.Surname = author.Surname.Trim();
            }
            _authors.Create(author);
        }

        public void Delete(int id)
        {
            var author = _authors.GetById(id);
            if (author is null) throw new InvalidOperationException("Author not found.");
            if (author.Books != null && author.Books.Any()) throw new InvalidOperationException("Author's have books.You hav not deleted this Author");
            _authors.Delete(id);
        }

        public List<Author> GetAll()
        {
            return _authors.GetAll();
        }

        public List<Book> GetBooksByAuthor(int authorId)
        {
            var author=_authors.GetById(authorId);
            return author?.Books.OrderBy(b=>b.Name).ToList() ?? new List<Book>();
        }

        public Author? GetByID(int id)
        {
            return _authors.GetById(id);
        }
    }
}
