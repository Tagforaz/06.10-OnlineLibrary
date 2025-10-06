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
            var actions = new Dictionary<int, (string Label, Action Act)>
            {
                [1] = ("Create Book", () => RunRightPane("Create Book", _manage.CreateBook)),
                [2] = ("Delete Book", () => RunRightPane("Delete Book", _manage.DeleteBook)),
                [3] = ("Get Book by Id", () => RunRightPane("Get Book by Id", _manage.GetBookById)),
                [4] = ("Show All Books", () => RunRightPane("All Books", _manage.ShowAllBooks)),
                [5] = ("Create Author", () => RunRightPane("Create Author", _manage.CreateAuthor)),
                [6] = ("Show All Authors", () => RunRightPane("All Authors", _manage.ShowAllAuthors)),
                [7] = ("Author's Books", () => RunRightPane("Author's Books", _manage.ShowAuthorsBooks)),
                [8] = ("Reserve Book", () => RunRightPane("Reserve Book", _manage.ReserveBook)),
                [9] = ("Reservation List", () => RunRightPane("Reservation List", _manage.ReservationList)),
                [10] = ("Change Reservation", () => RunRightPane("Change Reservation Status", _manage.ChangeReservationStatus)),
                [11] = ("User's Reservations", () => RunRightPane("User's Reservations", _manage.UsersReservationsList)),
            };

            RetroUi.Intro("Online Library");

            while (true)
            {
                RetroUi.ComputeLayout();
                RetroUi.Frame("Online Library");

                var menuItems = new List<string>();
                foreach (var kv in actions)
                    menuItems.Add($"{kv.Key,2}. {kv.Value.Label}");
                RetroUi.LeftMenu("Menu", menuItems);

                RetroUi.RightBegin("Welcome");
                RetroUi.RightWriteLines(new[]{
            "Use numbers [1-11] to navigate.",
            "Press 0 to Exit."
        });
                RetroUi.RightEnd("Enter your choice [0-11] and press ENTER...");

                RetroUi.Footer("Enter your choice [0-11]: ");
                Console.SetCursorPosition(RetroUi.L.Bottom.x1 + 25, RetroUi.L.Bottom.y1 + 1);
                var raw = (Console.ReadLine() ?? "").Trim();
                if (!int.TryParse(raw, out var pick))
                {
                    RetroUi.ErrorMsg("Please enter a number.");
                    System.Threading.Thread.Sleep(900);
                    continue;
                }
                if (pick == 0) { RetroUi.Info("Exiting..."); System.Threading.Thread.Sleep(500); Console.Clear(); return; }

                if (!actions.TryGetValue(pick, out var item))
                {
                    RetroUi.Warn("Wrong selection. Try again.");
                    System.Threading.Thread.Sleep(900);
                    continue;
                }

                try { item.Act(); }
                catch (Exception ex)
                {
                    RetroUi.ErrorMsg(ex.Message);
                    Console.ReadLine();
                }
            }
        }

        private static void RunRightPane(string title, Action action)
        {
            RetroUi.RightBegin(title);

            action.Invoke();

            RetroUi.RightEnd();
            RetroUi.RefreshFrameHeader("Online Library");
        }
    }
}



