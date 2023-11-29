using ApiMySQL.Model;

namespace ApiMySQL.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetMuscleGroupCategories(int id);
        Task<Category> GetCategory(int id);
        Task<bool> InsertCategory(Category category);
        Task<bool> UpdateCategory(Category category);
        Task<bool> DeleteCategory(int id);
    }
}
