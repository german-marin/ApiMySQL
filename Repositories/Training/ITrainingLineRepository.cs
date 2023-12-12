using ApiMySQL.Model;

namespace ApiMySQL.Repositories
{
    public interface ITrainingLineRepository
    {
        Task<IEnumerable<TrainingLine>> GetTrainingLinesOfTraining(int id);
        Task<TrainingLine> GetTrainingLine(int id);
        Task<bool> InsertTrainingLine(TrainingLine trainingLine);
        Task<bool> UpdateTrainingLine(TrainingLine trainingLine);
        Task<bool> DeleteTrainingLine(int id);
    }
}
