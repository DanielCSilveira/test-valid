using Infra.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repository
{
    public class OrderRepository : DataBaseUtils, IOrderRepository
    {
        private readonly AppDbContext _db;

        public OrderRepository(IConfiguration configuration
            ,AppDbContext dbContext) : base(configuration)
        {
            _db = dbContext;
        }

        public async Task<Order?> Get(Guid id)
        {
            return await _db.Orders.Include(p => p.Customer).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _db.Orders.Include(p => p.Customer).ToListAsync();
        }

        public async Task<Guid> Insert(Order order)
        {
            _db.Orders.Add(order);
            _db.SaveChanges();
            return await Task.FromResult(order.Id);
            
        }

        public async Task<int> Update(Order entity)
        {
            _db.Update(entity);
            return await _db.SaveChangesAsync(); 
        }

        public Task<int> UpdateStatusAsync(Guid orderId, string newStatus)
        {
            return ExecuteProcedureAsync("order_update_status", new
            {
                p_id = orderId,
                p_new_status = newStatus
            });
        }

    }
}
