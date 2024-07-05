using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMySQL.Repositories
{
    public class TrainingLineRepository : ITrainingLineRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TrainingLineRepository(IHttpContextAccessor httpContextAccessor)
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

        public async Task<IEnumerable<TrainingLine>> GetTrainingLinesOfTraining(int id)
        {
            return await DbContext.TrainingLines
                .Where(tl => tl.TrainingID == id)
                .ToListAsync();
        }

        public async Task<TrainingLine> GetTrainingLine(int id)
        {
            return await DbContext.TrainingLines.FindAsync(id);
        }

        public async Task<bool> InsertTrainingLine(TrainingLine trainingLine)
        {
            DbContext.TrainingLines.Add(trainingLine);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTrainingLine(TrainingLine trainingLine)
        {
            DbContext.Entry(trainingLine).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTrainingLine(int id)
        {
            var trainingLine = await DbContext.TrainingLines.FindAsync(id);
            if (trainingLine == null)
            {
                return false;
            }

            DbContext.TrainingLines.Remove(trainingLine);
            await DbContext.SaveChangesAsync();
            return true;
        }
    }
}
