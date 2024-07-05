using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMySQL.Repositories
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TrainingRepository(IHttpContextAccessor httpContextAccessor)
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

        public async Task<Training> GetTraining(int id)
        {
            return await DbContext.Trainings
                .Where(t => t.ID == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Training>> GetAllTrainings()
        {
            return await DbContext.Trainings.ToListAsync();
        }

        public async Task<int> InsertTraining(Training training)
        {
            DbContext.Trainings.Add(training);
            await DbContext.SaveChangesAsync();
            return training.ID;
        }

        public async Task<bool> UpdateTraining(Training training)
        {
            DbContext.Entry(training).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTraining(int id)
        {
            var training = await DbContext.Trainings.FindAsync(id);
            if (training == null)
            {
                return false;
            }

            DbContext.Trainings.Remove(training);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTrainingAndTrainingLines(int id)
        {
            var training = await DbContext.Trainings.FindAsync(id);
            if (training == null)
            {
                return false;
            }

            var trainingLines = await DbContext.TrainingLines
                .Where(tl => tl.TrainingID == id)
                .ToListAsync();

            DbContext.TrainingLines.RemoveRange(trainingLines);
            DbContext.Trainings.Remove(training);

            await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CustomerExist(int id)
        {
            var exists = await DbContext.Customers.AnyAsync(customer => customer.ID == id);
            return exists;
        }
    }
}
