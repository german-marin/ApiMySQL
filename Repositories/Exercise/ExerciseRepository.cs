using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMySQL.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExerciseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Exercise>> GetCategoryExercises(int id)
        {
            return await _context.Exercises
                .Where(e => e.CategoryID == id)
                .AsNoTracking() // Añade este método para evitar el rastreo
                .ToListAsync();
        }

        public async Task<Exercise> GetExercise(int id)
        {
            return await _context.Exercises.FindAsync(id);
        }

        public async Task<bool> InsertExercise(Exercise exercise)
        {
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateExercise(Exercise exercise)
        {
            //_context.Entry(exercise).State = EntityState.Modified;
            //await _context.SaveChangesAsync();
            //return true;
            // Verificar si ya hay una instancia rastreada del ejercicio y desacoplarla
            var local = _context.Exercises.Local.FirstOrDefault(e => e.ID == exercise.ID);
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(exercise).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteExercise(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null)
            {
                return false;
            }

            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
