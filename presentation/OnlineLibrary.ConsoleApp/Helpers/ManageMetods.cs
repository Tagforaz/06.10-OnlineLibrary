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
        private static bool IsLettersOnly(string s) => s.All(c => char.IsLetter(c) || c == ' ' || c == '-');
        public void CreateBook()
        {
            int step = 0;
            string name = "";
            int page = 0;
            int authorId = 0;

            while (true)
            {
                RetroUi.RightBegin("Create Book");

                if (!string.IsNullOrWhiteSpace(name)) RetroUi.RightWriteLine($"Name     : {name}");
                if (page > 0) RetroUi.RightWriteLine($"Page     : {page}");
                if (authorId > 0) RetroUi.RightWriteLine($"AuthorId : {authorId}");
                RetroUi.RightWriteLine("");

                if (step == 0)
                {
                    var input = (RetroUi.RightAsk("Book name (menu/back): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        RetroUi.RightError("Name must not be empty.");
                        RetroUi.RightEnd(); continue;
                    }
                    name = input;
                    step = 1;
                }
                else if (step == 1)
                {
                    var input = (RetroUi.RightAsk("Page count (menu/back): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); step = 0; continue; }

                    if (!int.TryParse(input, out page) || page <= 0)
                    {
                        RetroUi.RightError("Input a positive number.");
                        RetroUi.RightEnd(); continue;
                    }
                    step = 2;
                }
                else
                {
                    var authors = _authors.GetAll().OrderBy(a => a.Name).ToList();
                    if (authors.Count == 0)
                    {
                        RetroUi.RightWarn("No authors. Create an author first.");
                        RetroUi.RightEnd(); return;
                    }
                    RetroUi.RightWriteLine("-- Authors --");
                    foreach (var a in authors)
                    {
                        var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                        RetroUi.RightWriteLine($"ID:[{a.Id}] {full} | Books: {a.Books?.Count ?? 0}");
                    }
                    RetroUi.RightWriteLine("");

                    var input = (RetroUi.RightAsk("Author Id (menu/back): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); step = 1; continue; }

                    if (int.TryParse(input, out authorId) && _authors.GetByID(authorId) != null)
                    {
                        try
                        {
                            _books.Create(new Book { Name = name.Trim(), PageCount = page, AuthorId = authorId });
                            RetroUi.RightSuccess("Book created.");
                        }
                        catch (Exception ex)
                        {
                            RetroUi.RightError($"Error: {ex.Message}");
                        }
                        RetroUi.RightEnd(); return;
                    }
                    else
                    {
                        RetroUi.RightError("Author not found.");
                        RetroUi.RightEnd(); continue;
                    }
                }
                RetroUi.RightEnd();
            }
        }
        public void DeleteBook()
        {
            while (true)
            {
                RetroUi.RightBegin("Delete Book");

                var books = _books.GetAll().OrderBy(b => b.Name).ToList();
                if (books.Count == 0)
                {
                    RetroUi.RightWarn("No books.");
                    RetroUi.RightEnd(); return;
                }

                RetroUi.RightWriteLine("-- Books --");
                foreach (var b in books)
                {
                    var author = b.Author is null
                        ? $"AuthorId={b.AuthorId}"
                        : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                    RetroUi.RightWriteLine($"ID[{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{author}");
                }
                RetroUi.RightWriteLine("");

                var input = (RetroUi.RightAsk("Book Id (menu): ") ?? "").Trim();
                if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }

                if (!int.TryParse(input, out var id))
                {
                    RetroUi.RightError("Input a number.");
                    RetroUi.RightEnd(); continue;
                }

                var book = _books.GetById(id);
                if (book is null)
                {
                    RetroUi.RightError("Book not found.");
                    RetroUi.RightEnd(); continue;
                }

                try
                {
                    _books.Delete(id);
                    RetroUi.RightSuccess("Book deleted.");
                }
                catch (Exception ex)
                {
                    RetroUi.RightError($"Error: {ex.Message}");
                }

                RetroUi.RightEnd(); return;
            }
        }

        // 3) Get Book By Id
        public void GetBookById()
        {
            int step = 0;
            int id = 0;
            bool withHistory = false;
            Book? selected = null;

            while (true)
            {
                RetroUi.RightBegin("Get Book By Id");

                var books = _books.GetAll().OrderBy(b => b.Name).ToList();
                if (books.Count == 0)
                {
                    RetroUi.RightWarn("No books.");
                    RetroUi.RightEnd(); return;
                }

                RetroUi.RightWriteLine("-- Books --");
                foreach (var b in books)
                {
                    var authorName = b.Author is null
                        ? $"AuthorId={b.AuthorId}"
                        : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                    RetroUi.RightWriteLine($"ID[{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{authorName}");
                }
                RetroUi.RightWriteLine("");

                if (step == 0)
                {
                    var s = (RetroUi.RightAsk("Book Id (menu/back): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }

                    if (!int.TryParse(s, out id))
                    {
                        RetroUi.RightError("Input a number.");
                        RetroUi.RightEnd(); continue;
                    }

                    selected = _books.GetById(id, withDate: true);
                    if (selected is null)
                    {
                        RetroUi.RightError("Book not found.");
                        RetroUi.RightEnd(); continue;
                    }

                    step = 1;
                    RetroUi.RightEnd();
                    continue;
                }
                else
                {
                    var s = (RetroUi.RightAsk("Show reservation history? (y/n, menu/back): ") ?? "")
                        .Trim().ToLowerInvariant();
                    if (s == "menu") { RetroUi.RightEnd(); return; }
                    if (s == "back") { RetroUi.RightEnd(); step = 0; continue; }

                    if (s is "y" or "yes") withHistory = true;
                    else if (s is "n" or "no") withHistory = false;
                    else
                    {
                        RetroUi.RightError("Invalid choice. Use y/n.");
                        RetroUi.RightEnd(); continue;
                    }

                    var author = selected!.Author is null
                        ? $"AuthorId={selected.AuthorId}"
                        : (string.IsNullOrWhiteSpace(selected.Author.Surname) ? selected.Author.Name : $"{selected.Author.Name} {selected.Author.Surname}");

                    RetroUi.RightWriteLine($"[Book:{selected.Id}] Name:{selected.Name} | Pages:{selected.PageCount} | Author:{author}");

                    if (withHistory && selected.ReservedItems?.Any() == true)
                    {
                        RetroUi.RightWriteLine("-- History --");
                        foreach (var r in selected.ReservedItems.OrderByDescending(x => x.StartDate))
                        {
                            var bname = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                            RetroUi.RightWriteLine($"[ResID:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {bname} | {r.Status}");
                        }
                    }

                    RetroUi.RightEnd(); return;
                }
            }
        }
        public void ShowAllBooks()
        {
            RetroUi.RightBegin("All Books");
            var list = _books.GetAll();
            if (list.Count == 0)
            {
                RetroUi.RightWarn("No books.");
                RetroUi.RightEnd(); return;
            }
            foreach (var b in list.OrderBy(x => x.Name))
            {
                var author = b.Author is null
                    ? $"AuthorId={b.AuthorId}"
                    : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                RetroUi.RightWriteLine($"[BookID:{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{author}");
            }
            RetroUi.RightEnd();
        }
        public void CreateAuthor()
        {
            int step = 0;
            string name = "";
            string? surname = null;
            Gender gender = Gender.Unknown;

            while (true)
            {
                RetroUi.RightBegin("Create Author");
                if (!string.IsNullOrWhiteSpace(name)) RetroUi.RightWriteLine($"Name   : {name}");
                if (surname != null) RetroUi.RightWriteLine($"Surname: {surname}");
                RetroUi.RightWriteLine("");

                if (step == 0)
                {
                    var input = (RetroUi.RightAsk("Name (menu/back): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        RetroUi.RightError("Name must not be empty.");
                        RetroUi.RightEnd(); continue;
                    }
                    if (!IsLettersOnly(input))
                    {
                        RetroUi.RightError("Name must contain only letters.");
                        RetroUi.RightEnd(); continue;
                    }
                    name = input;
                    step = 1;
                }
                else if (step == 1)
                {
                    var input = (RetroUi.RightAsk("Surname (optional, menu/back): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (input.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); step = 0; continue; }

                    if (string.IsNullOrWhiteSpace(input))
                        surname = null;
                    else
                    {
                        if (!IsLettersOnly(input))
                        {
                            RetroUi.RightError("Surname must contain only letters.");
                            RetroUi.RightEnd(); continue;
                        }
                        surname = input;
                    }
                    step = 2;
                }
                else
                {
                    var s = (RetroUi.RightAsk("Gender (1-Unknown,2-Other,3-Male,4-Female) (menu/back): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); step = 1; continue; }

                    if (int.TryParse(s, out var n) && Enum.IsDefined(typeof(Gender), n)) gender = (Gender)n;
                    else if (Enum.TryParse<Gender>(s, true, out var g)) gender = g;
                    else
                    {
                        RetroUi.RightError("Wrong value. Use 1-4 or name.");
                        RetroUi.RightEnd(); continue;
                    }

                    var author = new Author { Name = name.Trim(), Surname = surname?.Trim(), Gender = gender };
                    try
                    {
                        _authors.Create(author);
                        RetroUi.RightSuccess("Author created.");
                    }
                    catch (Exception ex)
                    {
                        RetroUi.RightError($"Error: {ex.Message}");
                    }
                    RetroUi.RightEnd(); return;
                }

                RetroUi.RightEnd();
            }
        }
        public void DeleteAuthor()
        {
            while (true)
            {
                RetroUi.RightBegin("Delete Author");
                var authors = _authors.GetAll().OrderBy(a => a.Name).ToList();
                if (authors.Count == 0)
                {
                    RetroUi.RightWarn("No authors.");
                    RetroUi.RightEnd();
                    return;
                }
                RetroUi.RightWriteLine("-- Authors --");
                foreach (var a in authors)
                {
                    var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                    RetroUi.RightWriteLine($"[{a.Id}] {full} | Books: {a.Books?.Count ?? 0}");
                }
                RetroUi.RightWriteLine("");
                var input = (RetroUi.RightAsk("Author Id (menu): ") ?? "").Trim();
                if (input.Equals("menu", StringComparison.OrdinalIgnoreCase))
                {
                    RetroUi.RightEnd();
                    return;
                }
                if (!int.TryParse(input, out var id))
                {
                    RetroUi.RightError("Input a number.");
                    RetroUi.RightEnd();
                    continue;
                }
                var author = _authors.GetByID(id);
                if (author is null)
                {
                    RetroUi.RightError("Author not found.");
                    RetroUi.RightEnd();
                    continue;
                }
                if (author.Books != null && author.Books.Any())
                {
                    RetroUi.RightError("This author has books and cannot be deleted.");
                    foreach (var b in author.Books.OrderBy(b => b.Name))
                        RetroUi.RightWriteLine($" - {b.Name}");
                    RetroUi.RightEnd();
                    return;
                }

                try
                {
                    _authors.Delete(id);
                    RetroUi.RightSuccess("Author deleted.");
                }
                catch (Exception ex)
                {
                    RetroUi.RightError($"Error: {ex.Message}");
                }

                RetroUi.RightEnd();
                return;
            }
        }
        public void ShowAllAuthors()
        {
            RetroUi.RightBegin("All Authors");
            var list = _authors.GetAll();
            if (list.Count == 0)
            {
                RetroUi.RightWarn("No authors.");
                RetroUi.RightEnd(); return;
            }
            foreach (var a in list.OrderBy(x => x.Name))
            {
                var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                RetroUi.RightWriteLine($"ID:[{a.Id}] Name:{full} | Gender:{a.Gender} | Books: {a.Books?.Count ?? 0}");
            }
            RetroUi.RightEnd();
        }
        public void ShowAuthorsBooks()
        {
            while (true)
            {
                RetroUi.RightBegin("Author's Books");

                var authors = _authors.GetAll().OrderBy(a => a.Name).ToList();
                if (authors.Count == 0)
                {
                    RetroUi.RightWarn("No authors.");
                    RetroUi.RightEnd(); return;
                }

                RetroUi.RightWriteLine("-- Authors --");
                foreach (var a in authors)
                {
                    var full = string.IsNullOrWhiteSpace(a.Surname) ? a.Name : $"{a.Name} {a.Surname}";
                    RetroUi.RightWriteLine($"ID:[{a.Id}] {full} | Books: {a.Books?.Count ?? 0}");
                }
                RetroUi.RightWriteLine("");

                var s = (RetroUi.RightAsk("Author Id (menu): ") ?? "").Trim();
                if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }

                if (!int.TryParse(s, out var authorId))
                {
                    RetroUi.RightError("Wrong choice. Input a number.");
                    RetroUi.RightEnd(); continue;
                }

                var author = _authors.GetByID(authorId);
                if (author is null)
                {
                    RetroUi.RightError("Author not found.");
                    RetroUi.RightEnd(); continue;
                }

                var books = _authors.GetBooksByAuthor(authorId);
                if (books.Count == 0)
                {
                    RetroUi.RightWarn("This author has no books.");
                    RetroUi.RightEnd(); return;
                }

                RetroUi.RightWriteLine($"-- Books of {author.Name}{(string.IsNullOrWhiteSpace(author.Surname) ? "" : " " + author.Surname)} --");
                foreach (var b in books)
                {
                    var authorName = b.Author is null
                        ? $"AuthorId={b.AuthorId}"
                        : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                    RetroUi.RightWriteLine($"[BookID:{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{authorName}");
                }

                RetroUi.RightEnd(); return;
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
                RetroUi.RightBegin("Reserve Book");

                if (step == 0)
                {
                    var books = _books.GetAll().OrderBy(b => b.Name).ToList();
                    if (books.Count == 0)
                    {
                        RetroUi.RightWarn("No books.");
                        RetroUi.RightEnd(); return;
                    }

                    RetroUi.RightWriteLine("-- Books --");
                    foreach (var b in books)
                    {
                        var author = b.Author is null
                            ? $"AuthorId={b.AuthorId}"
                            : (string.IsNullOrWhiteSpace(b.Author.Surname) ? b.Author.Name : $"{b.Author.Name} {b.Author.Surname}");
                        RetroUi.RightWriteLine($"ID:[{b.Id}] Name:{b.Name} | Pages:{b.PageCount} | Author:{author}");
                    }
                    RetroUi.RightWriteLine("");

                    var s = (RetroUi.RightAsk("Book Id (menu/back): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }

                    if (!int.TryParse(s, out bookId))
                    {
                        RetroUi.RightError("Wrong choice. Input a number.");
                        RetroUi.RightEnd(); continue;
                    }
                    var book = _books.GetById(bookId);
                    if (book is null)
                    {
                        RetroUi.RightError("Book not found.");
                        RetroUi.RightEnd(); continue;
                    }

                    step = 1;
                    RetroUi.RightEnd();
                    continue;
                }
                else if (step == 1)
                {
                    var s = (RetroUi.RightAsk("FinCode (menu/back): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); step = 0; continue; }

                    if (string.IsNullOrWhiteSpace(s))
                    {
                        RetroUi.RightError("FinCode must not be empty.");
                        RetroUi.RightEnd(); continue;
                    }

                    fin = s;
                    step = 2;
                    RetroUi.RightEnd();
                    continue;
                }
                else if (step == 2)
                {
                    var s = (RetroUi.RightAsk("Start (yyyy-MM-dd) (menu/back): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); step = 1; continue; }

                    if (!DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
                    {
                        RetroUi.RightError("Use yyyy-MM-dd.");
                        RetroUi.RightEnd(); continue;
                    }
                    step = 3;
                    RetroUi.RightEnd();
                    continue;
                }
                else
                {
                    var s = (RetroUi.RightAsk("End (yyyy-MM-dd) (menu/back): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); step = 2; continue; }

                    if (!DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out end))
                    {
                        RetroUi.RightError("Use yyyy-MM-dd.");
                        RetroUi.RightEnd(); continue;
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
                        RetroUi.RightSuccess("Reservation created (Confirmed).");
                    }
                    catch (Exception ex)
                    {
                        RetroUi.RightError($"Error: {ex.Message}");
                    }
                    RetroUi.RightEnd(); return;
                }
            }
        }
        public void ReservationList()
        {
            RetroUi.RightBegin("Reservation List");

            var qw = (RetroUi.RightAsk("Filter by status (y/n or 'menu'): ") ?? "").Trim().ToLowerInvariant();
            if (qw == "menu") { RetroUi.RightEnd(); return; }

            System.Collections.Generic.List<ReservedItem> list;

            if (qw is "y" or "yes")
            {
                while (true)
                {
                    var s = (RetroUi.RightAsk("Status (1-Confirmed,2-Started,3-Completed,4-Canceled or 'menu'): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }

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

                    RetroUi.RightError("Invalid value.");
                }
            }
            else
            {
                list = _reservations.GetAll();
            }

            if (list.Count == 0)
            {
                RetroUi.RightWarn("No reservations.");
                RetroUi.RightEnd(); return;
            }

            RetroUi.RightWriteLine("");
            foreach (var r in list.OrderByDescending(x => x.StartDate))
            {
                var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                RetroUi.RightWriteLine($"[ResID:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
            }

            RetroUi.RightEnd();
        }
        public void ChangeReservationStatus()
        {
            int step = 0;
            int selectedId = 0;
            Status currentStatus = default;

            while (true)
            {
                if (step == 0)
                {
                    RetroUi.RightBegin("Change Reservation Status");

                    var list = _reservations.GetAll().OrderByDescending(r => r.StartDate).ToList();
                    if (list.Count == 0)
                    {
                        RetroUi.RightWarn("No reservations.");
                        RetroUi.RightEnd(); return;
                    }

                    RetroUi.RightWriteLine("-- Reservations --");
                    foreach (var r in list)
                    {
                        var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                        RetroUi.RightWriteLine($"ID:[{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
                    }
                    RetroUi.RightWriteLine("");

                    var s = (RetroUi.RightAsk("Reservation Id (menu/back): ") ?? "").Trim();
                    if (s.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (s.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }

                    if (!int.TryParse(s, out selectedId) || selectedId <= 0)
                    {
                        RetroUi.RightError("Invalid Id.");
                        RetroUi.RightEnd(); continue;
                    }

                    var selected = list.FirstOrDefault(r => r.Id == selectedId);
                    if (selected is null)
                    {
                        RetroUi.RightError("Reservation not found.");
                        RetroUi.RightEnd(); continue;
                    }

                    currentStatus = selected.Status; 
                    step = 1;
                    RetroUi.RightEnd();
                    continue;
                }
                else
                {
                    RetroUi.RightBegin("Change Reservation Status");
                    RetroUi.RightWriteLine($"Selected Reservation Id: {selectedId}");
                    RetroUi.RightWriteLine($"Current Status         : {currentStatus}");
                    RetroUi.RightWriteLine("");

                    var ns = (RetroUi.RightAsk("New Status (1-Confirmed,2-Started,3-Completed,4-Canceled) (menu/back): ") ?? "").Trim();
                    if (ns.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }
                    if (ns.Equals("back", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); step = 0; continue; }

                    Status newStatus;
                    if (int.TryParse(ns, out var n) && Enum.IsDefined(typeof(Status), n)) newStatus = (Status)n;
                    else if (Enum.TryParse<Status>(ns, true, out var e)) newStatus = e;
                    else
                    {
                        RetroUi.RightError("Invalid value.");
                        RetroUi.RightEnd(); continue;
                    }
                    if (newStatus == currentStatus)
                    {
                        RetroUi.RightError("No change: reservation is already in the selected status.");
                        RetroUi.RightEnd(); continue; 
                    }

                    try
                    {
                        _reservations.Update(selectedId, newStatus);
                        RetroUi.RightSuccess("Reservation status updated.");
                    }
                    catch (Exception ex)
                    {
                        RetroUi.RightError($"Error: {ex.Message}");
                    }

                    RetroUi.RightEnd(); return;
                }
            }
        }
        public void UsersReservationsList()
        {
            int step = 0;
            string fin = "";
            System.Collections.Generic.List<ReservedItem> list = null!;

            while (true)
            {
                if (step == 0)
                {
                    RetroUi.RightBegin("User's Reservations");
                    var input = (RetroUi.RightAsk("FinCode (menu): ") ?? "").Trim();
                    if (input.Equals("menu", StringComparison.OrdinalIgnoreCase)) { RetroUi.RightEnd(); return; }

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        RetroUi.RightError("Wrong input. Try again.");
                        RetroUi.RightEnd(); continue;
                    }

                    fin = input;
                    list = _reservations.GetByUser(fin);
                    if (list.Count == 0)
                    {
                        RetroUi.RightWarn("No reservations for this user. Try another FIN.");
                        RetroUi.RightEnd(); continue;
                    }

                    step = 1;
                    RetroUi.RightEnd();
                    continue;
                }
                else
                {
                    RetroUi.RightBegin($"Reservations of {fin}");
                    foreach (var r in list.OrderByDescending(x => x.StartDate))
                    {
                        var book = r.Book is null ? $"BookId={r.BookId}" : r.Book.Name;
                        RetroUi.RightWriteLine($"[ResID:{r.Id}] FIN:{r.FinCode} | {r.StartDate:yyyy-MM-dd} - {r.EndDate:yyyy-MM-dd} | {book} | {r.Status}");
                    }
                    RetroUi.RightEnd(); return;
                }
            }
        }
    }
}
