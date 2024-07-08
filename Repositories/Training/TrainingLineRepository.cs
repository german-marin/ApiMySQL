using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
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
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
            try
            {
                var trainingLines = await DbContext.TrainingLines
                    .Where(tl => tl.TrainingID == id)
                    .ToListAsync();

                Log.Logger.Information("Retrieved {Count} training lines for training ID {TrainingId}", trainingLines.Count, id);

                return trainingLines;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving training lines for training ID {TrainingId}", id);
                throw;
            }
        }

        public async Task<TrainingLine> GetTrainingLine(int id)
        {
            try
            {
                var trainingLine = await DbContext.TrainingLines.FindAsync(id);

                if (trainingLine == null)
                {
                    Log.Logger.Information("Training line with ID {Id} not found", id);
                }
                else
                {
                    Log.Logger.Information("Retrieved training line: {@TrainingLine}", trainingLine);
                }

                return trainingLine;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving training line with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> InsertTrainingLine(TrainingLine trainingLine)
        {
            try
            {
                DbContext.TrainingLines.Add(trainingLine);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Inserted training line with ID {Id}", trainingLine.ID);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error inserting training line {@TrainingLine}", trainingLine);
                throw;
            }
        }

        public async Task<bool> UpdateTrainingLine(TrainingLine trainingLine)
        {
            try
            {
                DbContext.Entry(trainingLine).State = EntityState.Modified;
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Updated training line with ID {Id}", trainingLine.ID);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error updating training line with ID {Id}", trainingLine.ID);
                throw;
            }
        }

        public async Task<bool> DeleteTrainingLine(int id)
        {
            try
            {
                var trainingLine = await DbContext.TrainingLines.FindAsync(id);
                if (trainingLine == null)
                {
                    Log.Logger.Information("Training line with ID {Id} not found for deletion", id);
                    return false;
                }

                DbContext.TrainingLines.Remove(trainingLine);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Deleted training line with ID {Id}", id);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error deleting training line with ID {Id}", id);
                throw;
            }
        }
    }
}
