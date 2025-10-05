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
            int step = 0;        
            string name = "";
            int page = 0;
            int authorId = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Create Book ===");
                if (!string.IsNullOrWhiteSpace(name)) Console.WriteLine($"Name: {name}");
                if (page > 0) Console.WriteLine($"Page: {page}");
                if (authorId > 0) Console.WriteLine($"AuthorId: {authorId}");
                Console.WriteLine();
                if (step == 0)
                {
                    Console.Write("Book name (back/menu): ");
                    var input = (Console.ReadLine() ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) return;   
                    if (!string.IsNullOrWhiteSpace(input)) { name = input; step = 1; }
                    else { Console.WriteLine("Name not be null"); Console.ReadKey(true); }
                }
                else if (step == 1)
                {
                    Console.Write("Page count (back/menu): ");
                    var input = (Console.ReadLine() ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) { step = 0; continue; }
                    if (int.TryParse(input, out page) && page > 0) { step = 2; }
                    else { Console.WriteLine("Input a positive number."); Console.ReadKey(true); }
                }
                else 
                {
                    var authors = _authors.GetAll().OrderBy(a => a.Name).ToList();
                    if (authors.Count == 0) { Console.WriteLine("You have not a authors.Create an author first."); Console.ReadKey(true); return; }
                    Console.WriteLine("-- Authors --");
                    foreach (var a in authors)
                    {
                        var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                        Console.WriteLine($"ID:[{a.Id}] {full} | Books: {a.Books?.Count ?? 0}");
                    }
                    Console.Write("\nAuthor Id (back/menu): ");
                    var input = (Console.ReadLine() ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) { step = 1; continue; }

                    if (int.TryParse(input, out authorId) && _authors.GetByID(authorId) != null)
                    {
                        _books.Create(new Book { Name = name.Trim(), PageCount = page, AuthorId = authorId });
                        Console.Clear();
                        Console.WriteLine("Book created.");
                        return;
                    }
                    else { Console.WriteLine("Author not found."); Console.ReadKey(true); }
                }
            }
        }
        public void DeleteBook()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Delete Book ===");
                var books = _books.GetAll().OrderBy(b => b.Name).ToList();
                if (books.Count == 0)
                {
                    Console.WriteLine("No books.");
                    Console.ReadKey(true);
                    return;
                }
                Console.WriteLine("-- Books --");
                foreach (var b in books)
                {
                    var author = b.Author is null
                        ? $"AuthorId={b.AuthorId}"
                        : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                    Console.WriteLine($"ID:[{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{author}");
                }
                Console.Write("\nBook Id (write 'menu' to return): ");
                var input = (Console.ReadLine() ?? "").Trim();
                if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                if (!int.TryParse(input, out var id))
                {
                    Console.WriteLine("Input a number.");
                    Console.ReadKey(true);
                    continue; 
                }
                var book = _books.GetById(id);
                if (book is null)
                {
                    Console.WriteLine("Book not found.");
                    Console.ReadKey(true);
                    continue;
                }
                try
                {
                    _books.Delete(id);
                    Console.Clear();
                    Console.WriteLine("Book deleted.");
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message);
                }
                Console.ReadKey(true);
                return;
            }
        }
        public void GetBookById()
        {
            int step = 0;         
            int id = 0;
            bool withHistory = false;
            Book? selected = null;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Get Book By Id ===");
                var books = _books.GetAll().OrderBy(b => b.Name).ToList();
                if (books.Count == 0)
                {
                    Console.WriteLine("No books.");
                    Console.ReadKey(true);
                    return;
                }
                Console.WriteLine("-- Books --");
                foreach (var b in books)
                {
                    var authorName = b.Author is null
                        ? $"AuthorId={b.AuthorId}"
                        : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                    Console.WriteLine($"ID:[{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{authorName}");
                }
                Console.WriteLine();
                if (step == 0)
                {
                    Console.Write("Book Id (back/menu): ");
                    var s = (Console.ReadLine() ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) return;
                    if (!int.TryParse(s, out id))
                    {
                        Console.WriteLine("Input a number.");
                        Console.ReadKey(true);
                        continue;
                    }
                    selected = _books.GetById(id, withDate: true);
                    if (selected is null)
                    {
                        Console.WriteLine("Book not found.");
                        Console.ReadKey(true);
                        continue;
                    }
                    step = 1;
                    continue;
                }
                else 
                {
                    Console.Write("Show reservation history? (y/n, back/menu): ");
                    var s = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
                    if (s == "menu") return;
                    if (s == "back") { step = 0; continue; }
                    if (s is "y" or "yes") withHistory = true;
                    else if (s is "n" or "no") withHistory = false;
                    else
                    {
                        Console.WriteLine("Invalid choice. Use y/n.");
                        Console.ReadKey(true);
                        continue;
                    }
                    var author = selected.Author is null
                        ? $"AuthorId={selected.AuthorId}"
                        : (string.IsNullOrWhiteSpace(selected.Author.Surname) ? selected.Author.Name : $"{selected.Author.Name} {selected.Author.Surname}");
                    Console.Clear();
                    Console.WriteLine("=== Get Book By Id ===");
                    Console.WriteLine($"[Book:{selected.Id}] Name:{selected.Name} | Pages:{selected.PageCount} | Author:{author}");
                    if (withHistory && selected.ReservedItems?.Any() == true)
                    {
                        Console.WriteLine("-- History --");
                        foreach (var r in selected.ReservedItems.OrderByDescending(x => x.StartDate))
                        {
                            var bname = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                            Console.WriteLine($"[ResID:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {bname} | {r.Status}");
                        }
                    }
                    Console.ReadKey(true);
                    return;
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
                Console.WriteLine($"[BookID:{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{author}");
            }
        }
        public void CreateAuthor()
        {
            int step = 0;                  
            string name = "";
            string? surname = null;
            Gender gender = Gender.Unknown;

            static bool IsLettersOnly(string s) =>
                s.All(c => char.IsLetter(c) || c == ' ' || c == '-');

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Create Author ===");
                if (!string.IsNullOrWhiteSpace(name)) Console.WriteLine($"Name   : {name}");
                if (surname != null) Console.WriteLine($"Surname: {surname}");
                Console.WriteLine();

                if (step == 0)
                {
                    Console.Write("Name (back/menu): ");
                    var input = (Console.ReadLine() ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) return;

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Name is not empty.");
                        Console.ReadKey(true);
                        continue;
                    }
                    if (!IsLettersOnly(input))
                    {
                        Console.WriteLine("Name must be letters only.");
                        Console.ReadKey(true);
                        continue;
                    }
                    name = input;
                    step = 1;
                }
                else if (step == 1)
                {
                    Console.Write("Surname (optional, back/menu): ");
                    var sn = Console.ReadLine() ?? "";
                    var input = sn.Trim();

                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) { step = 0; continue; }

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        surname = null; 
                    }
                    else
                    {
                        if (!IsLettersOnly(input))
                        {
                            Console.WriteLine("Surname must be letters only.");
                            Console.ReadKey(true);
                            continue;
                        }
                        surname = input;
                    }

                    step = 2;
                }
                else 
                {
                    Console.Write("Gender (Unknown=1, Other=2, Male=3, Female=4) (back/menu): ");
                    var s = (Console.ReadLine() ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { step = 1; continue; }

                    if (int.TryParse(s, out var n) && Enum.IsDefined(typeof(Gender), n))
                    {
                        gender = (Gender)n;
                    }
                    else if (Enum.TryParse<Gender>(s, true, out var g))
                    {
                        gender = g;
                    }
                    else
                    {
                        Console.WriteLine("Wrong value.Must be 1-4 or name.");
                        Console.ReadKey(true);
                        continue;
                    }
                    var author = new Author
                    {
                        Name = name.Trim(),
                        Surname = surname?.Trim(),
                        Gender = gender
                    };
                    try
                    {
                        _authors.Create(author);
                        Console.Clear();
                        Console.WriteLine("Author created.");
                    }
                    catch (Exception ex)
                    {
                        Console.Clear();
                        Console.WriteLine(ex.Message);
                    }
                    Console.ReadKey(true);
                    return;
                }
            }
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
                Console.WriteLine($"ID:[{a.Id}] {full} | {a.Gender} | Books: {a.Books?.Count ?? 0}");
            }
        }

        public void ShowAuthorsBooks()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Author's Books ===");
                var authors = _authors.GetAll().OrderBy(a => a.Name).ToList();
                if (authors.Count == 0)
                {
                    Console.WriteLine("No authors.");
                    Console.ReadKey(true);
                    return;
                }
                Console.WriteLine("-- Authors --");
                foreach (var a in authors)
                {
                    var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                    Console.WriteLine($"ID:[{a.Id}] {full} | Books: {a.Books?.Count ?? 0}");
                }
                Console.WriteLine();
                Console.Write("Author Id (write 'menu' to return): ");
                var s = (Console.ReadLine() ?? "").Trim();
                if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;

                if (!int.TryParse(s, out var authorId))
                {
                    Console.WriteLine("Wrong choice. Input a number.");
                    Console.ReadKey(true);
                    continue; 
                }
                var author = _authors.GetByID(authorId);
                if (author is null)
                {
                    Console.WriteLine("Author not found.");
                    Console.ReadKey(true);
                    continue; 
                }
                var books = _authors.GetBooksByAuthor(authorId);
                if (books.Count == 0)
                {
                    Console.WriteLine("This author has no books.");
                    Console.ReadKey(true);
                    return;
                }
                Console.Clear();
                Console.WriteLine($"=== Books of {author.Name}{(string.IsNullOrWhiteSpace(author.Surname) ? "" : " " + author.Surname)} ===");
                foreach (var b in books)
                {
                    var authorName = b.Author is null
                        ? $"AuthorId={b.AuthorId}"
                        : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");

                    Console.WriteLine($"[BookID:{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{authorName}");
                }
                Console.ReadKey(true);
                return;
            }
        }
        public void ReserveBook()
        {
            int step = 0;            
            int bookId = 0;
            string fin = "";
            DateTime start = default, end = default;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Reserve Book ===");

                if (step == 0)
                {
                    var books = _books.GetAll().OrderBy(b => b.Name).ToList();
                    if (books.Count == 0)
                    {
                        Console.WriteLine("No books.");
                        Console.ReadKey(true);
                        return;
                    }

                    Console.WriteLine("-- Books --");
                    foreach (var b in books)
                    {
                        var author = b.Author is null
                            ? $"AuthorId={b.AuthorId}"
                            : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                        Console.WriteLine($"ID:[{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{author}");
                    }
                    Console.WriteLine();

                    Console.Write("Book Id (back/menu): ");
                    var s = (Console.ReadLine() ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) return;

                    if (!int.TryParse(s, out bookId))
                    {
                        Console.WriteLine("Wrong choice.Input a number.");
                        Console.ReadKey(true);
                        continue;
                    }
                    var book = _books.GetById(bookId);
                    if (book is null)
                    {
                        Console.WriteLine("Book not found.");
                        Console.ReadKey(true);
                        continue;
                    }

                    step = 1;
                    continue;
                }
                else if (step == 1)
                {
                    Console.Write("FinCode (back/menu): ");
                    var s = (Console.ReadLine() ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { step = 0; continue; }

                    if (string.IsNullOrWhiteSpace(s))
                    {
                        Console.WriteLine("FinCode wasnt be null.");
                        Console.ReadKey(true);
                        continue;
                    }

                    fin = s;
                    step = 2;
                    continue;
                }
                else if (step == 2)
                {
                    Console.Write("Start (yyyy-MM-dd) (back/menu): ");
                    var s = (Console.ReadLine() ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { step = 1; continue; }

                    if (!DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
                    {
                        Console.WriteLine("Use yyyy-MM-dd.");
                        Console.ReadKey(true);
                        continue;
                    }
                    step = 3;
                    continue;
                }
                else 
                {
                    Console.Write("End (yyyy-MM-dd) (back/menu): ");
                    var s = (Console.ReadLine() ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { step = 2; continue; }

                    if (!DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out end))
                    {
                        Console.WriteLine("Use yyyy-MM-dd.");
                        Console.ReadKey(true);
                        continue;
                    }
                    var item = new ReservedItem
                    {
                        BookId = bookId,
                        FinCode = fin.Trim(),
                        StartDate = start,
                        EndDate = end
                    };
                    try
                    {
                        _reservations.Create(item);
                        Console.Clear();
                        Console.WriteLine("Reservation created (Confirmed).");
                    }
                    catch (Exception ex)
                    {
                        Console.Clear();
                        Console.WriteLine(ex.Message);
                    }
                    Console.ReadKey(true);
                    return;
                }
            }
        }
        public void ReservationList()
        {
            Console.Clear();
            Console.WriteLine("=== Reservation List ===");

            Console.Write("Filter by status (y/n or 'menu'): ");
            var qw = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            if (qw == "menu") return;
            List<ReservedItem> list;
            if (qw is "y" or "yes")
            {
                while (true)
                {
                    Console.Write("Status (Confirmed=1, Started=2, Completed=3, Canceled=4 or 'menu'): ");
                    var s = (Console.ReadLine() ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;

                    if (int.TryParse(s, out var n) && Enum.IsDefined(typeof(Status), n))
                    {
                        list = _reservations.GetAll((Status)n);
                        break;
                    }
                    if (Enum.TryParse<Status>(s, true, out var e))
                    {
                        list = _reservations.GetAll(e);
                        break;
                    }

                    Console.WriteLine("Invalid value.");
                    Console.ReadKey(true);
                    Console.Clear();
                    Console.WriteLine("=== Reservation List ===");
                    Console.WriteLine("(Filtering continues)");
                }
            }
            else
            {
                list = _reservations.GetAll();
            }
            if (list.Count == 0)
            {
                Console.WriteLine("No reservations.");
                Console.ReadKey(true);
                return;
            }
            Console.Clear();
            Console.WriteLine("=== Reservation List ===");
            foreach (var r in list.OrderByDescending(x => x.StartDate))
            {
                var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                Console.WriteLine($"[ResID:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
            }

            Console.ReadKey(true);
        }
        public void ChangeReservationStatus()
        {
            int step = 0;          
            int selectedId = 0;

            while (true)
            {
                if (step == 0)
                {
                    Console.Clear();
                    Console.WriteLine("=== Change Reservation Status ===");
                    var list = _reservations.GetAll().OrderByDescending(r => r.StartDate).ToList();
                    if (list.Count == 0)
                    {
                        Console.WriteLine("No reservations.");
                        Console.ReadKey(true);
                        return;
                    }
                    Console.WriteLine("-- Reservations --");
                    foreach (var r in list)
                    {
                        var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                        Console.WriteLine($"ID:[{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
                    }
                    Console.WriteLine();
                    Console.Write("Reservation Id (back/menu): ");
                    var s = (Console.ReadLine() ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) return; 

                    if (!int.TryParse(s, out selectedId) || selectedId <= 0)
                    {
                        Console.WriteLine("Enter a valid number.");
                        Console.ReadKey(true);
                        continue; 
                    }
                    var exists = list.Any(r => r.Id == selectedId);
                    if (!exists)
                    {
                        Console.WriteLine("Reservation not found.");
                        Console.ReadKey(true);
                        continue; 
                    }
                    step = 1; 
                    continue;
                }
                else 
                {
                    Console.Clear();
                    Console.WriteLine("=== Change Reservation Status ===");
                    Console.WriteLine($"Selected Reservation Id: {selectedId}");
                    Console.WriteLine();
                    Console.Write("New Status (Confirmed=1, Started=2, Completed=3, Canceled=4) (back/menu): ");
                    var ns = (Console.ReadLine() ?? "").Trim();
                    if (ns.Equals("menu", StringComparison.OrdinalIgnoreCase)) return;
                    if (ns.Equals("back", StringComparison.OrdinalIgnoreCase)) { step = 0; continue; }
                    Status newStatus;
                    if (int.TryParse(ns, out var n) && Enum.IsDefined(typeof(Status), n))
                    {
                        newStatus = (Status)n;
                    }
                    else if (Enum.TryParse<Status>(ns, true, out var e))
                    {
                        newStatus = e;
                    }
                    else
                    {
                        Console.WriteLine("Invalid value.");
                        Console.ReadKey(true);
                        continue; 
                    }
                    try
                    {
                        _reservations.Update(selectedId, newStatus);
                        Console.Clear();
                        Console.WriteLine("Reservation status updated.");
                    }
                    catch (Exception ex)
                    {
                        Console.Clear();
                        Console.WriteLine(ex.Message);
                    }
                    Console.ReadKey(true);
                    return; 
                }
            }
        }
        public void UsersReservationsList()
        {
            int step = 0;              
            string fin = "";
            List<ReservedItem> list = null!;
            while (true)
            {
                if (step == 0)
                {
                    Console.Clear();
                    Console.WriteLine("=== User's Reservations ===");
                    Console.Write("FinCode (write 'menu' to return): ");

                    var input = (Console.ReadLine() ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) return; 

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Wrong input.Try again");
                        Console.ReadKey(true);
                        continue; 
                    }
                    fin = input;
                    list = _reservations.GetByUser(fin);
                    if (list.Count == 0)
                    {
                        Console.WriteLine("No reservations for this user.Input another FinCode.");
                        Console.ReadKey(true);
                        continue; 
                    }
                    step = 1; 
                    continue;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine($"=== Reservations of {fin} ===");

                    foreach (var r in list.OrderByDescending(x => x.StartDate))
                    {
                        var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                        Console.WriteLine($"[ResID:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
                    }
                    Console.ReadKey(true);
                    return;
                }
            }
        }


    }

}
    
   



