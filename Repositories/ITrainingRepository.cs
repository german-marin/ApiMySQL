using ApiMySQL.Model;

namespace ApiMySQL.Repositories
{
    public interface ITrainingRepository
    {
        Task<Training> GetTraining(int id);
        Task<int> InsertTraining(Training training);
        Task<bool> UpdateTraining(Training training);
        Task<bool> DeleteTraining(int id);

    }
}
