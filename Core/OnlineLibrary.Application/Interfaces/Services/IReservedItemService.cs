using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Application.Interfaces.Services
{
    public interface IReservedItemService
    {
        void Create (ReservedItem item);
        void Delete(int id);
        List<ReservedItem> GetAll(Status? status = null);   
        void Update(int reservationId, Status newStatus);
        List<ReservedItem> GetByUser(string finCode);

    }
}
