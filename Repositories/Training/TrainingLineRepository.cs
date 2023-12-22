using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMySQL.Repositories
{
    public class TrainingLineRepository : ITrainingLineRepository
    {
        private readonly ApplicationDbContext _context;

        public TrainingLineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrainingLine>> GetTrainingLinesOfTraining(int id)
        {
            return await _context.TrainingLines
                .Where(tl => tl.TrainingID == id)
                .ToListAsync();
        }

        public async Task<TrainingLine> GetTrainingLine(int id)
        {
            return await _context.TrainingLines.FindAsync(id);
        }

        public async Task<bool> InsertTrainingLine(TrainingLine trainingLine)
        {
            _context.TrainingLines.Add(trainingLine);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTrainingLine(TrainingLine trainingLine)
        {
            _context.Entry(trainingLine).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTrainingLine(int id)
        {
            var trainingLine = await _context.TrainingLines.FindAsync(id);
            if (trainingLine == null)
            {
                return false;
            }

            _context.TrainingLines.Remove(trainingLine);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
