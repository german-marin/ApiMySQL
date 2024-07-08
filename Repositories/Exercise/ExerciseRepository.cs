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
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExerciseRepository(IHttpContextAccessor httpContextAccessor)
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

        public async Task<IEnumerable<Exercise>> GetCategoryExercises(int id)
        {
            try
            {
                var exercises = await DbContext.Exercises
                    .Where(e => e.CategoryID == id)
                    .AsNoTracking() // Añade este método para evitar el rastreo
                    .ToListAsync();

                Log.Logger.Information("Retrieved {Count} exercises for Category ID {CategoryId}", exercises.Count, id);

                return exercises;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving exercises for Category ID {CategoryId}", id);
                throw;
            }
        }

        public async Task<Exercise> GetExercise(int id)
        {
            try
            {
                var exercise = await DbContext.Exercises.FindAsync(id);

                if (exercise == null)
                {
                    Log.Logger.Information("Exercise with ID {Id} not found", id);
                }
                else
                {
                    Log.Logger.Information("Retrieved exercise: {@Exercise}", exercise);
                }

                return exercise;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving exercise with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> InsertExercise(Exercise exercise)
        {
            try
            {
                DbContext.Exercises.Add(exercise);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Inserted exercise with ID {Id}", exercise.ID);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error inserting exercise {@Exercise}", exercise);
                throw;
            }
        }

        public async Task<bool> UpdateExercise(Exercise exercise)
        {
            try
            {
                // Verificar si ya hay una instancia rastreada del ejercicio y desacoplarla
                var local = DbContext.Exercises.Local.FirstOrDefault(e => e.ID == exercise.ID);
                if (local != null)
                {
                    DbContext.Entry(local).State = EntityState.Detached;
                }

                DbContext.Entry(exercise).State = EntityState.Modified;
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Updated exercise with ID {Id}", exercise.ID);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error updating exercise with ID {Id}", exercise.ID);
                throw;
            }
        }

        public async Task<bool> DeleteExercise(int id)
        {
            try
            {
                var exercise = await DbContext.Exercises.FindAsync(id);
                if (exercise == null)
                {
                    Log.Logger.Information("Exercise with ID {Id} not found for deletion", id);
                    return false;
                }

                DbContext.Exercises.Remove(exercise);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Deleted exercise with ID {Id}", id);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error deleting exercise with ID {Id}", id);
                throw;
            }
        }
    }
}
