using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Application.Interfaces.Services
{
    public interface IAuthorService
    {
        void Create (string name,string? surname,Gender gender);
        void Delete(int id);
        List<Author> GetAll();
        List<Book> GetBooksByAuthor(int authorId);
        Author? GetByID(int id);

       
    }
}
