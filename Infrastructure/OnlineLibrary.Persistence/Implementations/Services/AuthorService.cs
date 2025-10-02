using OnlineLibrary.Application.Interfaces.Repositories;
using OnlineLibrary.Application.Interfaces.Services;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
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
        public void Create(string name,string? surname,Gender gender)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("Author name is used");
            var author = new Author
            {
                Name = name.Trim(),
                Surname = string.IsNullOrWhiteSpace(surname) ? null: surname.Trim(),
                Gender = gender
            };
            _authors.Create(author);
        }

        public void Delete(int id)
        {
            var author  = _authors.GetById(id);
            if (author == null) throw new InvalidOperationException("Author dont find");
            if (author.Books.FirstOrDefault()!=null) throw new InvalidOperationException("Author have books.Delete impossible");
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
