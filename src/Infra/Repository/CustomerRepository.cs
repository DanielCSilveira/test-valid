using Infra.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repository
{
    public class CustomerRepository : DataBaseUtils, ICustomerRepository
    {
        public CustomerRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Customer?> Get(Guid id)
        {
            var customer = await this.QueryProcedureAsync<Customer>("customer_get", new { id = id });
            return customer.FirstOrDefault();
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await this.QueryProcedureAsync<Customer>("customer_list", new { });
        }

        public async Task<Guid> Insert(Customer customer)
        {
            var parameters = new
            {
                p_name = customer.Name,
                p_email = customer.Email,
            };

            var ret = await this.QueryProcedureAsync<Guid>("customer_create", parameters);
            return ret.FirstOrDefault();

        }

    
    }
}
