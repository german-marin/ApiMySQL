using ApiMySQL.Data;
using ApiMySQL.Model;
using Dapper;
using MySql.Data.MySqlClient;

namespace ApiMySQL.Repositories
{
    public class MuscleGroupRepository : IMuscleGroupRepository

        {
            private readonly MySQLConfiguration _connectionString;

            public MuscleGroupRepository(MySQLConfiguration connectionString)
            {
                _connectionString = connectionString;
            }

            protected MySqlConnection DbConnection()
            {
                return new MySqlConnection(_connectionString.ConnectionString);
            }

            public async Task<bool> DeleteMuscleGroup(int id)
            {
                var db = DbConnection();

                var sql = @"DELETE FROM grupo_muscular
                        WHERE ID_grupo = @Id ";
                var result = await db.ExecuteAsync(sql, new { Id = id });

                return result > 0;
            }

            public async Task<IEnumerable<MuscleGroup>> GetAllMuscleGroup()
            {
              var db = DbConnection();
            
            //Averiguar otra manera de hacer esto sin tener que poner alias a las columnas
                var sql = @"SELECT ID_grupo as ID, descripcion as Description, Imagen_frente as ImageFront, Imagen_trasera as ImageRear
                        FROM grupo_muscular";

                return await db.QueryAsync<MuscleGroup>(sql, new { });
            }

            public async Task<MuscleGroup> GetMuscleGroup(int id)
            {
                var db = DbConnection();
            //Averiguar otra manera de hacer esto sin tener que poner alias a las columnas
            var sql = @"SELECT ID_grupo as ID, descripcion as Description, Imagen_frente as ImageFront, Imagen_trasera as ImageRear
                        FROM grupo_muscular
                        WHERE ID_grupo = @Id ";

                return await db.QueryFirstOrDefaultAsync<MuscleGroup>(sql, new { Id = id });
            }

            public async Task<bool> InsertMuscleGroup(MuscleGroup muscleGroup)
            {
                var db = DbConnection();

                var sql = @"INSERT INTO grupo_muscular(descripcion, Imagen_frente, Imagen_trasera, f_ult_act)
                       VALUES(@Description, @ImageFront, @ImageRear, CURRENT_TIMESTAMP)";

                var result = await db.ExecuteAsync(sql, new { muscleGroup.Description, muscleGroup.ImageFront, muscleGroup.ImageRear });

                return result > 0;

            }

            public async Task<bool> UpdateMuscleGroup(MuscleGroup muscleGroup)
            {
                var db = DbConnection();

                var sql = @"UPDATE grupo_muscular
                            SET  descripcion = @Description, 
                                 Imagen_frente = @ImageFront, 
                                 Imagen_trasera = @ImageRear, 
                                 f_ult_act = CURRENT_TIMESTAMP
                            WHERE ID_grupo = @Id ";

                var result = await db.ExecuteAsync(sql, new { muscleGroup.Description, muscleGroup.ImageFront, muscleGroup.ImageRear, Id = muscleGroup.ID });

                return result > 0;
            }
        }
    }
