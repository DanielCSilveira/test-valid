using Infra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repository
{
    public interface IOrderRepository : IBaseRepository<Order, Guid>
    {
        Task<int> UpdateStatusAsync(Guid orderId, string newStatus);
    }
}
