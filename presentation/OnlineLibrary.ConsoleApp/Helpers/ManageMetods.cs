using OnlineLibrary.Application.Interfaces.Services;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Enums;
using OnlineLibrary.Persistence.Implementations.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.ConsoleApp.Helpers
{
    public class ManageMetods
    {
        private readonly IAuthorService _authors;
        private readonly IBookService _books;
        private readonly IReservedItemService _reservations;

        public ManageMetods(IAuthorService authors, IBookService books, IReservedItemService reservations)
        {
            _authors = authors;
            _books = books;
            _reservations = reservations;
        }
        public void CreateBook()
        {
            Console.Clear();
            Console.WriteLine("=== Create Book ===");
            string name; 
            do 
            { 
                Console.Write("Book name: ");
                name = Console.ReadLine(); 
            }
            while(string.IsNullOrWhiteSpace(name));
            int page;
            while (true) 
            {
                Console.Write("Page count: ");
                if (int.TryParse(Console.ReadLine(), out page)) 
                break;
                Console.WriteLine("Input a number.");
            }
            int authorId;
            while (true)
            {
                ShowAllAuthors();
                Console.Write("Author Id: ");
                if (int.TryParse(Console.ReadLine(), out authorId)) 
                    break;
                Console.WriteLine("Input a number."); 
            }

            var book = new Book 
            {
                Name = name.Trim(), 
                PageCount = page,
                AuthorId = authorId 
            };
            _books.Create(book);
            Console.WriteLine("Book created.");
        }
        public void DeleteBook()
        {
            Console.Clear();
            Console.WriteLine("=== Delete Book ===");
            var books = _books.GetAll().OrderBy(b => b.Name).ToList();
            if (books.Count == 0)
            {
                Console.WriteLine("No books.");
                return;
            }
            Console.WriteLine("-- Books --");
            foreach (var b in books)
            {
                var author = b.Author is null ? $"AuthorId={b.AuthorId}" :
                    (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                Console.WriteLine($"[{b.Id}] {b.Name} | Pages:{b.PageCount} | Author:{author}");
            }
            Console.WriteLine();
            int id;
            while (true)
            { 
                Console.Write("Book Id: ");
                if (int.TryParse(Console.ReadLine(), out id)) 
                    break;
             Console.WriteLine("Input a number."); 
            }
            _books.Delete(id);
            Console.WriteLine("Book deleted.");
        }

        public void GetBookById()
        {
            Console.Clear();
            Console.WriteLine("=== Get Book By Id ===");
            var books = _books.GetAll().OrderBy(b => b.Name).ToList();
            if (books.Count == 0) { Console.WriteLine("No books."); return; }

            Console.WriteLine("-- Books --");
            foreach (var b in books)
            {
                var authorName = b.Author is null
                    ? $"AuthorId={b.AuthorId}"
                    : (string.IsNullOrWhiteSpace(b.Author.Surname)
                        ? b.Author.Name
                        : $"{b.Author.Name} {b.Author.Surname}");

                Console.WriteLine($"[{b.Id}] {b.Name} | Pages:{b.PageCount} | Author:{authorName}");
            }
            int id;
            while (true)
            {
                Console.Write("Book Id: ");
                if (int.TryParse(Console.ReadLine(), out id))
                    break;
                Console.WriteLine("Input a number.");
            }
            Console.Write("Show reservation history (yes/no): ");
            bool withHistory = ((Console.ReadLine() ?? "").Trim().ToLowerInvariant()) is "y" or "yes";

            var book = _books.GetById(id, withHistory);
            if (book is null) 
            {
                Console.WriteLine("Book not found.");
                return;
            }
            var author = book.Author is null
                ? $"AuthorId={book.AuthorId}"
                : (string.IsNullOrWhiteSpace(book.Author.Surname) ? book.Author.Name : $"{book.Author.Name} {book.Author.Surname}");
            Console.WriteLine($"[Book:{book.Id}] {book.Name} | Pages:{book.PageCount} | Author:{author}");

            if (withHistory && book.ReservedItems?.Any() == true)
            {
                Console.WriteLine("-- History --");
                foreach (var r in book.ReservedItems.OrderByDescending(x => x.StartDate))
                {
                    var bname = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                    Console.WriteLine($"[Res:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} → {r.EndDate:yyyy-MM-dd} | {bname} | {r.Status}");
                }
            }
        }
        public void ShowAllBooks()
        {
            Console.Clear();
            Console.WriteLine("=== All Books ===");

            var list = _books.GetAll();
            if (list.Count == 0) 
            {
                Console.WriteLine("No books.");
                return;
            }
            foreach (var b in list.OrderBy(x => x.Name))
            {
                var author = b.Author is null
                    ? $"AuthorId={b.AuthorId}"
                    : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                Console.WriteLine($"[Book:{b.Id}] {b.Name} | Pages:{b.PageCount} | Author:{author}");
            }
        }
        public void CreateAuthor()
        {
            Console.Clear();
            Console.WriteLine("=== Create Author ===");
            string name;
            do 
            {
                Console.Write("Name: ");
                name = Console.ReadLine();
            } 
            while (string.IsNullOrWhiteSpace(name));
            Console.Write("Surname (optional): ");
            var surname = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(surname)) surname = null;
            Gender gender;
            while (true)
            {
                Console.Write("Gender (Unknown=1, Other=2, Male=3, Female=4): ");
                var s = Console.ReadLine();
                if (int.TryParse(s, out var n) && Enum.IsDefined(typeof(Gender), n)) 
                {
                    gender = (Gender)n; 
                    break; 
                }
                if (Enum.TryParse<Gender>(s, true, out var g)) 
                {
                    gender = g;
                    break; 
                }
                Console.WriteLine("Invalid value.");
            }

            var author = new Author { Name = name.Trim(), Surname = surname?.Trim(), Gender = gender };
            _authors.Create(author);
            Console.WriteLine("Author created.");
        }

        public void ShowAllAuthors()
        {
            Console.Clear();
            Console.WriteLine("=== All Authors ===");
            var list = _authors.GetAll();
            if (list.Count == 0) 
            {
                Console.WriteLine("No authors.");
                return;
            }

            foreach (var a in list.OrderBy(x => x.Name))
            {
                var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                Console.WriteLine($"[{a.Id}] {full} | {a.Gender} | Books: {a.Books?.Count ?? 0}");
            }
        }
        public void ShowAuthorsBooks()
        {
            Console.Clear();
            Console.WriteLine("=== Author's Books ===");
            var authors = _authors.GetAll().OrderBy(a => a.Name).ToList();
            if (authors.Count == 0) { Console.WriteLine("No authors."); return; }

            Console.WriteLine("-- Authors --");
            foreach (var a in authors)
            {
                var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                Console.WriteLine($"[{a.Id}] {full} | Books: {a.Books?.Count ?? 0}");
            }
            Console.WriteLine();
            int authorId;
            while (true) 
            {
                Console.Write("Author Id: ");
                if (int.TryParse(Console.ReadLine(), out authorId)) break;
                Console.WriteLine("Input a number.");
            }
            var books = _authors.GetBooksByAuthor(authorId);
            if (books.Count == 0)
            { 
                Console.WriteLine("No books.");
                return;
            }
            foreach (var b in books)
            {
                var author = b.Author is null
                    ? $"AuthorId={b.AuthorId}"
                    : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                Console.WriteLine($"[Book:{b.Id}] {b.Name} | Pages:{b.PageCount} | Author:{author}");
            }
        }
        public void ReserveBook()
        {
            Console.Clear();
            Console.WriteLine("=== Reserve Book ===");
            var books = _books.GetAll().OrderBy(b => b.Name).ToList();
            if (books.Count == 0) { Console.WriteLine("No books."); return; }

            Console.WriteLine("-- Books --");
            foreach (var b in books)
            {
                var author = b.Author is null ? $"AuthorId={b.AuthorId}" :
                    (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                Console.WriteLine($"[{b.Id}] {b.Name} | Pages:{b.PageCount} | Author:{author}");
            }
            Console.WriteLine();
            int bookId; 
            while (true)
            {
                Console.Write("Book Id: ");
                if (int.TryParse(Console.ReadLine(), out bookId)) break; 
                Console.WriteLine("Input a number."); 
            }
            var book = _books.GetById(bookId);  
            if (book is null)
            {
                Console.WriteLine("Book not found.");
                return;
            }
            string fin;
            do
            { Console.Write("FinCode: ");
                fin = Console.ReadLine(); 
            }
            while (string.IsNullOrWhiteSpace(fin));

            DateTime start;
            while (true)
            {
                Console.Write("Start (yyyy-MM-dd): ");
                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out start)) break;
                Console.WriteLine("Use yyyy-MM-dd.");
            }

            DateTime end;
            while (true)
            {
                Console.Write("End   (yyyy-MM-dd): ");
                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out end)) break;
                Console.WriteLine("Use yyyy-MM-dd.");
            }

            var item = new ReservedItem 
            {
                BookId = bookId,
                FinCode = fin.Trim(),
                StartDate = start,
                EndDate = end 
            };
            _reservations.Create(item);
            Console.WriteLine("Reservation created (Confirmed).");
        }
        public void ReservationList()
        {
            Console.Clear();
            Console.WriteLine("=== Reservation List ===");
            Console.Write("Filter by status (y/n): ");
            var ans = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            var list = ans is "y" or "yes"
                ? _reservations.GetAll(ReadStatus())
                : _reservations.GetAll();
            if (list.Count == 0) { Console.WriteLine("No reservations."); return; }
            foreach (var r in list.OrderByDescending(x => x.StartDate))
            {
                var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                Console.WriteLine($"[Res:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} → {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
            }
            Status ReadStatus()
            {
                while (true)
                {
                    Console.Write("Status (Confirmed=1, Started=2, Completed=3, Canceled=4): ");
                    var s = Console.ReadLine();
                    if (int.TryParse(s, out var n) && Enum.IsDefined(typeof(Status), n)) return (Status)n;
                    if (Enum.TryParse<Status>(s, true, out var e)) return e;
                    Console.WriteLine("Invalid value.");
                }
            }
        }
        public void ChangeReservationStatus()
        {
            Console.Clear();
            Console.WriteLine("=== Change Reservation Status ===");
            var list = _reservations.GetAll().OrderByDescending(r => r.StartDate).ToList();
            if (list.Count == 0) { Console.WriteLine("No reservations."); return; }

            Console.WriteLine("-- Reservations --");
            foreach (var r in list)
            {
                var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                Console.WriteLine($"[{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} → {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
            }
            Console.WriteLine();
            int id; 
            while (true)
            {
                Console.Write("Reservation Id: ");
                if (int.TryParse(Console.ReadLine(), out id)&& id>0) break;
                Console.WriteLine("Enter a number."); 
            }
            var exists = _reservations.GetAll().Any(r => r.Id == id);
            if (!exists)
            {
                Console.WriteLine("Reservation not found.");
                return;
            }
            Status st;
            while (true)
            {
                Console.Write("New Status (Confirmed=1, Started=2, Completed=3, Canceled=4): ");
                var s = Console.ReadLine();
                if (int.TryParse(s, out var n) && Enum.IsDefined(typeof(Status), n)) { st = (Status)n; break; }
                if (Enum.TryParse<Status>(s, true, out var e)) { st = e; break; }
                Console.WriteLine("Invalid value.");
            }
            _reservations.Update(id, st);
            Console.WriteLine("Reservation status updated.");
        }
        public void UsersReservationsList()
        {
            Console.Clear();
            Console.WriteLine("=== User's Reservations ===");
            string fin;
            do 
            {
                Console.Write("FinCode: "); 
                fin = Console.ReadLine();
            } 
            while (string.IsNullOrWhiteSpace(fin));
            var list = _reservations.GetByUser(fin.Trim());
            if (list.Count == 0) 
            {
                Console.WriteLine("No reservations for this user.");
                return; 
            }

            foreach (var r in list.OrderByDescending(x => x.StartDate))
            {
                var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                Console.WriteLine($"[Res:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} → {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
            }
        }


    }

}
    
   



