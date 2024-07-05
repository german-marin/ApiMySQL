using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
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
            _httpContextAccessor = httpContextAccessor;
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
            return await DbContext.Categories
                .Where(c => c.MuscleGroupID == id)
                .ToListAsync();
        }

        public async Task<Category> GetCategory(int id)
        {
            return await DbContext.Categories.FindAsync(id);
        }

        public async Task<bool> InsertCategory(Category category)
        {
            DbContext.Categories.Add(category);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            DbContext.Entry(category).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await DbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            DbContext.Categories.Remove(category);
            await DbContext.SaveChangesAsync();
            return true;
        }
    }
}
