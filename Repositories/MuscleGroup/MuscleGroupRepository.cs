using ApiMySQL.Data;
using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMySQL.Repositories
{
    public class MuscleGroupRepository : IMuscleGroupRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MuscleGroupRepository(IHttpContextAccessor httpContextAccessor)
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

        public async Task<IEnumerable<MuscleGroup>> GetAllMuscleGroup()
        {
            return await DbContext.MuscleGroups.ToListAsync();
        }

        public async Task<MuscleGroup> GetMuscleGroup(int id)
        {
            return await DbContext.MuscleGroups.FindAsync(id);
        }

        public async Task<bool> InsertMuscleGroup(MuscleGroup muscleGroup)
        {
            DbContext.MuscleGroups.Add(muscleGroup);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateMuscleGroup(MuscleGroup muscleGroup)
        {
            DbContext.Entry(muscleGroup).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMuscleGroup(int id)
        {
            var muscleGroup = await DbContext.MuscleGroups.FindAsync(id);
            if (muscleGroup == null)
            {
                return false;
            }

            DbContext.MuscleGroups.Remove(muscleGroup);
            await DbContext.SaveChangesAsync();
            return true;
        }
    }
}
