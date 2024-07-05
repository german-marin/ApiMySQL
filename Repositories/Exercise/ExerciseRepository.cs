using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
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
            _httpContextAccessor = httpContextAccessor;
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
            return await DbContext.Exercises
                .Where(e => e.CategoryID == id)
                .AsNoTracking() // Añade este método para evitar el rastreo
                .ToListAsync();
        }

        public async Task<Exercise> GetExercise(int id)
        {
            return await DbContext.Exercises.FindAsync(id);
        }

        public async Task<bool> InsertExercise(Exercise exercise)
        {
            DbContext.Exercises.Add(exercise);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateExercise(Exercise exercise)
        {
            // Verificar si ya hay una instancia rastreada del ejercicio y desacoplarla
            var local = DbContext.Exercises.Local.FirstOrDefault(e => e.ID == exercise.ID);
            if (local != null)
            {
                DbContext.Entry(local).State = EntityState.Detached;
            }

            DbContext.Entry(exercise).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteExercise(int id)
        {
            var exercise = await DbContext.Exercises.FindAsync(id);
            if (exercise == null)
            {
                return false;
            }

            DbContext.Exercises.Remove(exercise);
            await DbContext.SaveChangesAsync();
            return true;
        }
    }
}
