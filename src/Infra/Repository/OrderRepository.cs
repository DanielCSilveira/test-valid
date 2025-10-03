using Infra.Model;
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
        public OrderRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Order?> Get(Guid id)
        {
            var order = await this.QueryProcedureAsync<Order>("order_get", new { id = id });
            return order.FirstOrDefault();
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            return await this.QueryProcedureAsync<Order>("order_get", new { });
        }

        public async Task<Guid> Insert(Order order)
        {
            var parameters = new
            {
                p_customer_id = order.CustomerId,
                p_amount = order.Amount
            };

            var ret = await this.QueryProcedureAsync<Guid>("order_create", parameters);
            return ret.FirstOrDefault();
            
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
