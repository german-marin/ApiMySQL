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
    public class MuscleGroupRepository : IMuscleGroupRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MuscleGroupRepository(IHttpContextAccessor httpContextAccessor)
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

        public async Task<IEnumerable<MuscleGroup>> GetAllMuscleGroup()
        {
            try
            {
                var muscleGroups = await DbContext.MuscleGroups.ToListAsync();

                Log.Logger.Information("Retrieved {Count} muscle groups", muscleGroups.Count);

                return muscleGroups;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving all muscle groups");
                throw;
            }
        }

        public async Task<MuscleGroup> GetMuscleGroup(int id)
        {
            try
            {
                var muscleGroup = await DbContext.MuscleGroups.FindAsync(id);

                if (muscleGroup == null)
                {
                    Log.Logger.Information("Muscle group with ID {Id} not found", id);
                }
                else
                {
                    Log.Logger.Information("Retrieved muscle group: {@MuscleGroup}", muscleGroup);
                }

                return muscleGroup;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error retrieving muscle group with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> InsertMuscleGroup(MuscleGroup muscleGroup)
        {
            try
            {
                DbContext.MuscleGroups.Add(muscleGroup);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Inserted muscle group with ID {Id}", muscleGroup.ID);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error inserting muscle group {@MuscleGroup}", muscleGroup);
                throw;
            }
        }

        public async Task<bool> UpdateMuscleGroup(MuscleGroup muscleGroup)
        {
            try
            {
                DbContext.Entry(muscleGroup).State = EntityState.Modified;
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Updated muscle group with ID {Id}", muscleGroup.ID);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error updating muscle group with ID {Id}", muscleGroup.ID);
                throw;
            }
        }

        public async Task<bool> DeleteMuscleGroup(int id)
        {
            try
            {
                var muscleGroup = await DbContext.MuscleGroups.FindAsync(id);
                if (muscleGroup == null)
                {
                    Log.Logger.Information("Muscle group with ID {Id} not found for deletion", id);
                    return false;
                }

                DbContext.MuscleGroups.Remove(muscleGroup);
                await DbContext.SaveChangesAsync();

                Log.Logger.Information("Deleted muscle group with ID {Id}", id);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error deleting muscle group with ID {Id}", id);
                throw;
            }
        }
    }
}
