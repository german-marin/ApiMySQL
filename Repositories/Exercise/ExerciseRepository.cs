using ApiMySQL.Data;
using ApiMySQL.Model;
using Dapper;
using MySql.Data.MySqlClient;

namespace ApiMySQL.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly MySQLConfiguration _connectionString;

        public ExerciseRepository(MySQLConfiguration connectionString)
        {
            _connectionString = connectionString;
        }

        protected MySqlConnection DbConnection()
        {
            return new MySqlConnection(_connectionString.ConnectionString);
        }

        public async Task<IEnumerable<Exercise>> GetCategoryExercises(int id)
        {
            var db = DbConnection();

            var sql = @"SELECT ID_ejercicio as ID, descripcion as Description, ID_categoria_FK as IdCategory, Imagen as Image
                        FROM ejercicios
                        WHERE ID_categoria_FK = @Id ";

            return await db.QueryAsync<Exercise>(sql, new { Id = id });
        }

        public async Task<Exercise> GetExercise(int id)
        {
            var db = DbConnection();
            var sql = @"SELECT ID_ejercicio as ID, descripcion as Description, ID_categoria_FK as IdCategory, Imagen as Image
                        FROM ejercicios
                        WHERE ID_ejercicio = @Id ";

            return await db.QueryFirstOrDefaultAsync<Exercise>(sql, new { Id = id });
        }

        public async Task<bool> InsertExercise(Exercise exercise)
        {
            var db = DbConnection();

            var sql = @"INSERT INTO ejercicios(descripcion, ID_categoria_FK, Imagen, f_ult_act)
                       VALUES(@Description, @IdCategory, @Image, CURRENT_TIMESTAMP)";

            var result = await db.ExecuteAsync(sql, new { exercise.Description, exercise.IdCategory, exercise.Image });

            return result > 0;

        }

        public async Task<bool> UpdateExercise(Exercise exercise)
        {
            var db = DbConnection();

            var sql = @"UPDATE ejercicios
                          SET  descripcion = @Description, 
                               ID_categoria_FK = @IdCategory, 
                               Imagen = @Image,
                               f_ult_act = CURRENT_TIMESTAMP
                         WHERE ID_ejercicio = @Id ";

            var result = await db.ExecuteAsync(sql, new { exercise.Description, exercise.IdCategory, exercise.Image, Id = exercise.ID });

            return result > 0;
        }
        public async Task<bool> DeleteExercise(int id)
        {
            var db = DbConnection();

            var sql = @"DELETE FROM ejercicios
                        WHERE ID_ejercicio = @Id ";
            var result = await db.ExecuteAsync(sql, new { Id = id });

            return result > 0;
        }
    }
}
