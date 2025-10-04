using Infra.Model;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _db;

        public CustomerRepository(IConfiguration configuration, AppDbContext dbContext) : base(configuration)
        {
            _db = dbContext;
        }

        public async Task<Customer?> Get(Guid id)
        {
            return await _db.Customers.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Customer?> GetByMail(string mail)
        {
            return await _db.Customers.FirstOrDefaultAsync(i => i.Email == mail);

        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await _db.Customers.Where(c => c.Active).ToListAsync();
                       
        }

        public async Task<Guid> Insert(Customer customer)
        {
            
            _db.Customers.Add(customer);
            _db.SaveChanges();

            return await Task.FromResult(customer.Id);


        }


    }
}
