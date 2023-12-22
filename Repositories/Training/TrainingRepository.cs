using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMySQL.Repositories
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly ApplicationDbContext _context;

        public TrainingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Training> GetTraining(int id)
        {
            return await _context.Trainings
                .Where(t => t.ID == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Training>> GetAllTrainings()
        {
            return await _context.Trainings.ToListAsync();
        }

        public async Task<int> InsertTraining(Training training)
        {
            _context.Trainings.Add(training);
            await _context.SaveChangesAsync();
            return training.ID;
        }

        public async Task<bool> UpdateTraining(Training training)
        {
            _context.Entry(training).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTraining(int id)
        {
            var training = await _context.Trainings.FindAsync(id);
            if (training == null)
            {
                return false;
            }

            _context.Trainings.Remove(training);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTrainingAndTrainingLines(int id)
        {
            var training = await _context.Trainings.FindAsync(id);
            if (training == null)
            {
                return false;
            }

            var trainingLines = await _context.TrainingLines
                .Where(tl => tl.TrainingID == id)
                .ToListAsync();

            _context.TrainingLines.RemoveRange(trainingLines);
            _context.Trainings.Remove(training);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CustomerExist(int id)
        {
            var exists = await _context.Customers.AnyAsync(customer => customer.ID == id);

            return exists;
        }
    }
}
