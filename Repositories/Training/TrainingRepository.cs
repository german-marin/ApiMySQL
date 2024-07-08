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
    public class TrainingRepository : ITrainingRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TrainingRepository(IHttpContextAccessor httpContextAccessor)
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

        public async Task<Training> GetTraining(int id)
        {
            try
            {
                var training = await DbContext.Trainings
                    .Where(t => t.ID == id)
                    .FirstOrDefaultAsync();

                if (training == null)
                {
                    Log.Logger.Information("Training with ID {Id} not found", id);
                }
                else
                {
                    Log.Logger.Information("Retrieved training: {@Training}", training);
                }

                return training;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving training with ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Training>> GetAllTrainings()
        {
            try
            {
                var trainings = await DbContext.Trainings.ToListAsync();

                Log.Logger.Information("Retrieved {Count} trainings", trainings.Count);

                return trainings;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving all trainings");
                throw;
            }
        }

        public async Task<int> InsertTraining(Training training)
        {
            try
            {
                DbContext.Trainings.Add(training);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Inserted training with ID {Id}", training.ID);

                return training.ID;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error inserting training {@Training}", training);
                throw;
            }
        }

        public async Task<bool> UpdateTraining(Training training)
        {
            try
            {
                DbContext.Entry(training).State = EntityState.Modified;
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Updated training with ID {Id}", training.ID);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error updating training with ID {Id}", training.ID);
                throw;
            }
        }

        public async Task<bool> DeleteTraining(int id)
        {
            try
            {
                var training = await DbContext.Trainings.FindAsync(id);
                if (training == null)
                {
                    Log.Logger.Information("Training with ID {Id} not found for deletion", id);
                    return false;
                }

                DbContext.Trainings.Remove(training);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Deleted training with ID {Id}", id);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error deleting training with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteTrainingAndTrainingLines(int id)
        {
            try
            {
                var training = await DbContext.Trainings.FindAsync(id);
                if (training == null)
                {
                    Log.Logger.Information("Training with ID {Id} not found for deletion", id);
                    return false;
                }

                var trainingLines = await DbContext.TrainingLines
                    .Where(tl => tl.TrainingID == id)
                    .ToListAsync();

                DbContext.TrainingLines.RemoveRange(trainingLines);
                DbContext.Trainings.Remove(training);

                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Deleted training with ID {Id} and {Count} training lines", id, trainingLines.Count);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error deleting training with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> CustomerExist(int id)
        {
            try
            {
                var exists = await DbContext.Customers.AnyAsync(customer => customer.ID == id);
                return exists;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error checking if customer exists with ID {Id}", id);
                throw;
            }
        }
    }
}
