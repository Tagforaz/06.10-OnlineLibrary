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
    public class ReservedItemService : IReservedItemService
    {
        private readonly IReservedItemRepository _reservations;
        private readonly IBookRepository _books;
        public ReservedItemService(IReservedItemRepository reservations,IBookRepository books)
        {
            _reservations = reservations;
            _books = books;
        }
        public void Create(int bookId, string finCode, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(finCode)) throw new ArgumentException("Fincode  wrong");
            finCode = finCode.Trim().ToUpperInvariant();
            if (endDate <= startDate) throw new ArgumentException("End date must higher than start date");
            var book = _books.GetById(bookId);
            if (book == null) throw new ArgumentException("Book is not found");
            int activeForUser = _reservations.GetAll().Count(r => string.Equals(r.FinCode, finCode, StringComparison.OrdinalIgnoreCase)
            && (r.Status == Status.Confirmed || r.Status == Status.Started));
            if (activeForUser >= 3)  throw new InvalidOperationException("You reserv max 3 book!!!.");
            bool hasActive = _reservations.GetAll().Any(r =>r.BookId == bookId &&(r.Status == Status.Confirmed || r.Status == Status.Started) && startDate < r.EndDate && endDate > r.StartDate);
            if (hasActive) throw new InvalidOperationException("Book also reserved for this date");
            var item = new ReservedItem
            {
                BookId = bookId,
                Book = book,
                StartDate = startDate,
                EndDate = endDate,
                FinCode = finCode,
                Status = Status.Confirmed
            };
            _reservations.Create(item);

        }

        public void Delete(int id)
        {
            var item = _reservations.GetById(id);
            if (item is null) return;
             if (item.Status == Status.Started) throw new InvalidOperationException("Reserved books not deleted");
            _reservations.Delete(id);
        }

        public List<ReservedItem> GetAll(Status? status = null)
        {
             var rb = _reservations.GetAll().AsQueryable();
            if (status.HasValue) rb = rb.Where(r => r.Status == status.Value);
            return rb.OrderByDescending(r => r.StartDate).ToList();
        }

        public List<ReservedItem> GetByUser(string finCode)
        {
            finCode = finCode?.Trim().ToUpperInvariant() ?? string.Empty;
            return _reservations.GetAll().Where(r => r.FinCode.ToUpper() == finCode).OrderByDescending(r => r.StartDate).ToList();
        }

        public void Update(int reservationId, Status newStatus)
        {
            var item = _reservations.GetById(reservationId);
            if (item is null) return;
            if (item.Status == newStatus) return;
            bool allowed = false;
            if (item.Status == Status.Confirmed)
            {
                allowed = (newStatus == Status.Started || newStatus == Status.Canceled);
            }
            else if (item.Status == Status.Started)
            {
                allowed = (newStatus == Status.Completed || newStatus == Status.Canceled);
            }
            else
            {
                allowed = false;
            }
            if (!allowed)
                throw new InvalidOperationException("Yolverilməz status keçidi.");
            item.Status = newStatus;
            _reservations.Update(item);
        }
    }
}
