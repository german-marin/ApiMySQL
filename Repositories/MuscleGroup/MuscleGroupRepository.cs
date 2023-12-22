using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMySQL.Repositories
{
    public class MuscleGroupRepository : IMuscleGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public MuscleGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MuscleGroup>> GetAllMuscleGroup()
        {
            return await _context.MuscleGroups.ToListAsync();
        }

        public async Task<MuscleGroup> GetMuscleGroup(int id)
        {
            return await _context.MuscleGroups.FindAsync(id);
        }

        public async Task<bool> InsertMuscleGroup(MuscleGroup muscleGroup)
        {
            _context.MuscleGroups.Add(muscleGroup);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateMuscleGroup(MuscleGroup muscleGroup)
        {
            _context.Entry(muscleGroup).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMuscleGroup(int id)
        {
            var muscleGroup = await _context.MuscleGroups.FindAsync(id);
            if (muscleGroup == null)
            {
                return false;
            }

            _context.MuscleGroups.Remove(muscleGroup);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
