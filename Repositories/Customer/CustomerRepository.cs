using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMySQL.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ApplicationDbContext DbContext
        {
            get
            {
                return _httpContextAccessor.HttpContext.Items["DbContext"] as ApplicationDbContext;
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            return await DbContext.Customers.ToListAsync();
        }

        public async Task<Customer> GetCustomer(int id)
        {
            return await DbContext.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task<bool> InsertCustomer(Customer customer)
        {
            DbContext.Customers.Add(customer);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            DbContext.Entry(customer).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCustomer(int id)
        {
            var customer = await DbContext.Customers.FindAsync(id);
            if (customer == null)
            {
                return false;
            }

            DbContext.Customers.Remove(customer);
            await DbContext.SaveChangesAsync();
            return true;
        }
    }
}
