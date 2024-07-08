using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
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
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
            try
            {
                var customers = await DbContext.Customers.ToListAsync();

                Log.Logger.Information("Retrieved {Count} customers", customers.Count);

                return customers;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving customers");
                throw;
            }
        }

        public async Task<Customer> GetCustomer(int id)
        {
            try
            {
                var customer = await DbContext.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.ID == id);

                if (customer == null)
                {
                    Log.Logger.Information("Customer with ID {Id} not found", id);
                }
                else
                {
                    Log.Logger.Information("Retrieved customer: {@Customer}", customer);
                }

                return customer;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving customer with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> InsertCustomer(Customer customer)
        {
            try
            {
                DbContext.Customers.Add(customer);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Inserted customer with ID {Id}", customer.ID);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error inserting customer {@Customer}", customer);
                throw;
            }
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            try
            {
                DbContext.Entry(customer).State = EntityState.Modified;
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Updated customer with ID {Id}", customer.ID);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error updating customer with ID {Id}", customer.ID);
                throw;
            }
        }

        public async Task<bool> DeleteCustomer(int id)
        {
            try
            {
                var customer = await DbContext.Customers.FindAsync(id);
                if (customer == null)
                {
                    Log.Logger.Information("Customer with ID {Id} not found for deletion", id);
                    return false;
                }

                DbContext.Customers.Remove(customer);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Deleted customer with ID {Id}", id);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error deleting customer with ID {Id}", id);
                throw;
            }
        }
    }
}
