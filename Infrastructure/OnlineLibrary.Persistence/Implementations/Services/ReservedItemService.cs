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
        public void Create(ReservedItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item), "Reservation wasn't be null");
            if (string.IsNullOrWhiteSpace(item.FinCode)) throw new ArgumentException("FinCode wasnt't be null", nameof(item.FinCode));
            item.FinCode = item.FinCode.Trim();
            if (item.EndDate <= item.StartDate) throw new ArgumentException("End date must be greater than Start date");
            var book = _books.GetById(item.BookId);
            if (book is null) throw new InvalidOperationException("Book not found");
            int activeReserv=_reservations.GetAll().Count(r=>string.Equals(r.FinCode,item.FinCode,StringComparison.OrdinalIgnoreCase)&&(r.Status==Status.Confirmed||r.Status==Status.Started));
            if (activeReserv >= 3) throw new InvalidOperationException("User mus be reserv max 3 book");
            bool hasReserv = _reservations.GetAll().Any(r=>r.BookId==item.BookId&&(r.Status==Status.Confirmed||r.Status==Status.Started)&&item.StartDate<r.EndDate&&item.EndDate>r.StartDate);
            if (hasReserv) throw new InvalidOperationException("Book is reserved for this dates");
            item.Status=Status.Confirmed;
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
            if (status.HasValue)
            {
                rb = rb.Where(r => r.Status == status.Value);
            }
            return rb.OrderByDescending(r => r.StartDate).ToList();
        }

        public List<ReservedItem> GetByUser(string finCode)
        {
            var fin = finCode?.Trim() ?? string.Empty;

            return _reservations.GetAll().Where(r => string.Equals(r.FinCode, fin, StringComparison.OrdinalIgnoreCase)).OrderByDescending(r => r.StartDate).ToList();
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
                throw new InvalidOperationException("Invalid status");
            item.Status = newStatus;
            _reservations.Update(item);
        }
    }
}
