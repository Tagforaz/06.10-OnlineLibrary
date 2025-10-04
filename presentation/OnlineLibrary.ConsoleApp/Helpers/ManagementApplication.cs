using OnlineLibrary.Application.Interfaces.Services;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Persistence.Contexts;
using OnlineLibrary.Persistence.Implementations.Repositories;
using OnlineLibrary.Persistence.Implementations.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.ConsoleApp.Helpers
{
    public class ManagementApplication
    {
        private readonly ManageMetods _manage;
        public ManagementApplication()
        {
            var db = new AppDbContext();
            var authorRepo = new AuthorRepository(db);
            var bookRepo = new BookRepository(db);
            var resRepo = new ReservedItemRepository(db);
            IAuthorService authorService = new AuthorService(authorRepo);
            IBookService bookService = new BookService(bookRepo, authorRepo);
            IReservedItemService reserveService = new ReservedItemService(resRepo, bookRepo);
            _manage = new ManageMetods(authorService, bookService, reserveService);
        }
        public void Run()
        {

            var actions = new Dictionary<int, Action>
            {
                [1] = _manage.CreateBook,
                [2] = _manage.DeleteBook,
                [3] = _manage.GetBookById,
                [4] = _manage.ShowAllBooks,
                [5] = _manage.CreateAuthor,
                [6] = _manage.ShowAllAuthors,
                [7] = _manage.ShowAuthorsBooks,
                [8] = _manage.ReserveBook,
                [9] = _manage.ReservationList,
                [10] = _manage.ChangeReservationStatus,
                [11] = _manage.UsersReservationsList
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Online Library");
                Console.WriteLine("====================================");
                Console.WriteLine("1) Create Book");
                Console.WriteLine("2) Delete Book");
                Console.WriteLine("3) Get Book by Id");
                Console.WriteLine("4) Show All Books");
                Console.WriteLine("5) Create Author");
                Console.WriteLine("6) Show All Authors");
                Console.WriteLine("7) Author's Books");
                Console.WriteLine("8) Reserve Book");
                Console.WriteLine("9) Reservation List");
                Console.WriteLine("10) Change Reservation Status");
                Console.WriteLine("11) User's Reservations");
                Console.WriteLine("0) Exit");
                Console.Write("Select: ");

                if (!int.TryParse(Console.ReadLine(), out var pick))
                {
                    Console.WriteLine("Please enter a number.");
                    Console.Write("Press ENTER to continue..."); Console.ReadLine();
                    continue;
                }

                if (pick == 0)
                {
                    Console.WriteLine("Program ended.");
                    return;
                }

                try
                {
                    if (actions.TryGetValue(pick, out var act))
                    {
                        act();
                    }
                    else
                    {
                        Console.WriteLine("Wrong selection. Try again.");
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine();
                Console.Write("Press ENTER to continue...");
                Console.ReadLine();
            }
        }
    }
    }

 
