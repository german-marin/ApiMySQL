using ApiMySQL.Model;

namespace ApiMySQL.Repositories
{
    public interface IExerciseRepository
    {
        Task<IEnumerable<Exercise>> GetCategoryExercises(int id);
        Task<Exercise> GetExercise(int id);
        Task<bool> InsertExercise(Exercise exercise);
        Task<bool> UpdateExercise(Exercise exercise);
        Task<bool> DeleteExercise(int id);
    }
}
