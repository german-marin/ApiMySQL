using ApiMySQL.Data;
using ApiMySQL.Model;
using Dapper;
using MySql.Data.MySqlClient;
namespace ApiMySQL.Repositories
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly MySQLConfiguration _connectionString;

        public TrainingRepository(MySQLConfiguration connectionString)
        {
            _connectionString = connectionString;
        }

        protected MySqlConnection DbConnection()
        {
            return new MySqlConnection(_connectionString.ConnectionString);
        }   

        public async Task<Training> GetTraining(int id)
        {
            var db = DbConnection();
            var sql = @"SELECT ID_entrenamiento as ID, descripcion as Description, 
                        fecha_ini as StartDate, fecha_fin as EndDate, ID_cliente_FK as IdClient, notas as Notes
                        FROM entrenamiento
                        WHERE ID_entrenamiento = @Id ";

            return await db.QueryFirstOrDefaultAsync<Training>(sql, new { Id = id });
        }

        public async Task<int> InsertTraining(Training training)
        {
            var db = DbConnection();

            var sql = @"INSERT INTO entrenamiento(descripcion, fecha_ini, fecha_fin, ID_cliente_FK, notas, f_ult_act)
                       VALUES(@Description, @StartDate, @EndDate, @IdClient, @Notes, CURRENT_TIMESTAMP);
                       SELECT LAST_INSERT_ID();";


            return await db.QueryFirstOrDefaultAsync<int>(sql, new { training.Description, training.StartDate, training.EndDate, training.IdClient, training.Notes });
        }

        public async Task<bool> UpdateTraining(Training training)
        {
            var db = DbConnection();

            var sql = @"UPDATE entrenamiento
                          SET  descripcion = @Description, 
                               fecha_ini = @StartDate, 
                               fecha_fin = @EndDate, 
                               ID_cliente_FK = @IdClient, 
                               notas = @Notes, 
                               f_ult_act = CURRENT_TIMESTAMP
                         WHERE ID_entrenamiento = @Id ";

            var result = await db.ExecuteAsync(sql, new { training.Description, training.StartDate, training.EndDate, training.IdClient, training.Notes, Id = training.ID });

            return result > 0;
        }
        public async Task<bool> DeleteTraining(int id)
        {
            var db = DbConnection();

            var sql = @"DELETE FROM entrenamiento
                        WHERE ID_entrenamiento = @Id ";
            var result = await db.ExecuteAsync(sql, new { Id = id });

            return result > 0;
        }
        
    }
}
