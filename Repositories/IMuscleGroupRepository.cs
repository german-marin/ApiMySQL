using ApiMySQL.Model;

namespace ApiMySQL.Repositories
{
    public interface IMuscleGroupRepository
    {

        Task<IEnumerable<MuscleGroup>> GetAllMuscleGroup();
        Task<MuscleGroup> GetMuscleGroup(int id);
        Task<bool> InsertMuscleGroup(MuscleGroup muscleGroup);
        Task<bool> UpdateMuscleGroup(MuscleGroup muscleGroup);
        Task<bool> DeleteMuscleGroup(int id);
    }

}
