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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryRepository(IHttpContextAccessor httpContextAccessor)
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

        public async Task<IEnumerable<Category>> GetMuscleGroupCategories(int id)
        {
            try
            {
                var categories = await DbContext.Categories
                    .Where(c => c.MuscleGroupID == id)
                    .ToListAsync();

                Log.Logger.Information("Retrieved {Count} categories for MuscleGroupID {Id}", categories.Count, id);

                return categories;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving categories for MuscleGroupID {Id}", id);
                throw;
            }
        }

        public async Task<Category> GetCategory(int id)
        {
            try
            {
                var category = await DbContext.Categories.FindAsync(id);

                if (category == null)
                {
                    Log.Logger.Information("Category with ID {Id} not found", id);
                }
                else
                {
                    Log.Logger.Information("Retrieved category: {@Category}", category);
                }

                return category;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving category with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> InsertCategory(Category category)
        {
            try
            {
                DbContext.Categories.Add(category);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Inserted category with ID {Id}", category.ID);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error inserting category {@Category}", category);
                throw;
            }
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            try
            {
                DbContext.Entry(category).State = EntityState.Modified;
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Updated category with ID {Id}", category.ID);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error updating category with ID {Id}", category.ID);
                throw;
            }
        }

        public async Task<bool> DeleteCategory(int id)
        {
            try
            {
                var category = await DbContext.Categories.FindAsync(id);
                if (category == null)
                {
                    Log.Logger.Information("Category with ID {Id} not found for deletion", id);
                    return false;
                }

                DbContext.Categories.Remove(category);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Deleted category with ID {Id}", id);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error deleting category with ID {Id}", id);
                throw;
            }
        }
    }
}
