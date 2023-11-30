using ApiMySQL.Data;
using ApiMySQL.Model;
using Dapper;
using MySql.Data.MySqlClient;

namespace ApiMySQL.Repositories
{
    public class TrainingLineRepository : ITrainingLineRepository
    {
        private readonly MySQLConfiguration _connectionString;

        public TrainingLineRepository(MySQLConfiguration connectionString)
        {
            _connectionString = connectionString;
        }

        protected MySqlConnection DbConnection()
        {
            return new MySqlConnection(_connectionString.ConnectionString);
        }

        public async Task<IEnumerable<TrainingLine>> GetTrainingLinesOfTraining(int id)
        {
            var db = DbConnection();

            var sql = @"SELECT ID_lineas as ID, ID_ejercicio_FK as IdExercise, ID_entrenamiento_FK as IdTraining
                               series as Series, repeticiones as Repetition, pesos as Weight, recuperacion as Recovery, 
                               otros as Weight, notas as Notes
                        FROM lineas_entrenamiento
                        WHERE ID_entrenamiento_FK = @Id ";

            return await db.QueryAsync<TrainingLine>(sql, new { Id = id });
        }

        public async Task<TrainingLine> GetTrainingLine(int id)
        {
            var db = DbConnection();
            var sql = @"SELECT ID_lineas as ID, ID_ejercicio_FK as IdExercise, ID_entrenamiento_FK as IdTraining,
                               series as Series, repeticiones as Repetition, pesos as Weight, recuperacion as Recovery, 
                               otros as Weight, notas as Notes
                        FROM lineas_entrenamiento
                        WHERE ID_lineas = @Id ";

            return await db.QueryFirstOrDefaultAsync<TrainingLine>(sql, new { Id = id });
        }

        public async Task<bool> InsertTrainingLine(TrainingLine trainingLine)
        {
            var db = DbConnection();

            var sql = @"INSERT INTO lineas_entrenamiento(ID_ejercicio_FK, ID_entrenamiento_FK, series, repeticiones, pesos, recuperacion, otros, notas, f_ult_act)
                       VALUES(@IdExercise, @IdTraining, @Series, @Repetition, @Weight, @Recovery, @Others, @Notes, CURRENT_TIMESTAMP)";
            

            var result = await db.ExecuteAsync(sql, new { trainingLine.IdExercise, trainingLine.IdTraining, trainingLine.Series, trainingLine.Repetition, trainingLine.Weight, trainingLine.Recovery,trainingLine.Others, trainingLine.Notes });

            return result > 0;

        }

        public async Task<bool> UpdateTrainingLine(TrainingLine trainingLine)
        {
            var db = DbConnection();

            var sql = @"UPDATE lineas_entrenamiento
                          SET  ID_ejercicio_FK = @IdExercise,
                               ID_entrenamiento_FK = @IdTraining,
                               series = @Series,
                               repeticiones = @Repetition,
                               pesos = @Weight,
                               recuperacion = @Recovery,
                               otros = @Weight,
                               notas = @Notes,
                               f_ult_act = CURRENT_TIMESTAMP
                         WHERE ID_lineas = @Id ";

            var result = await db.ExecuteAsync(sql, new { trainingLine.IdExercise, trainingLine.IdTraining, trainingLine.Series, trainingLine.Repetition, trainingLine.Weight, trainingLine.Recovery, trainingLine.Others, trainingLine.Notes, Id = trainingLine.ID });

            return result > 0;
        }
        public async Task<bool> DeleteTrainingLine(int id)
        {
            var db = DbConnection();

            var sql = @"DELETE FROM lineas_entrenamiento
                              WHERE ID_lineas = @Id ";
            var result = await db.ExecuteAsync(sql, new { Id = id });

            return result > 0;
        }
    }
}