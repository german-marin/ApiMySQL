using ApiMySQL.Data;
using ApiMySQL.Model;
using Dapper;
using MySql.Data.MySqlClient;

namespace ApiMySQL.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MySQLConfiguration _connectionString;

        public CategoryRepository(MySQLConfiguration connectionString)
        {
            _connectionString = connectionString;
        }

        protected MySqlConnection DbConnection()
        {
            return new MySqlConnection(_connectionString.ConnectionString);
        }
               
        public async Task<IEnumerable<Category>> GetMuscleGroupCategories(int id)
        {
            var db = DbConnection();
            
            var sql = @"SELECT ID_categoria as ID, Descripcion as Description, ID_grupo_muscular_FK as IdMuscleGroup
                        FROM categoria
                        WHERE ID_grupo_muscular_FK = @Id ";

            return await db.QueryAsync<Category>(sql, new { Id = id });
        }

        public async Task<Category> GetCategory(int id)
        {
            var db = DbConnection();
            var sql = @"SELECT ID_categoria as ID, descripcion as Description, ID_grupo_muscular_FK as IdMuscleGroup
                        FROM categoria
                        WHERE ID_categoria = @Id ";

            return await db.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
        }

        public async Task<bool> InsertCategory(Category category)
        {
            var db = DbConnection();

            var sql = @"INSERT INTO categoria(descripcion, ID_grupo_muscular_FK, f_ult_act)
                       VALUES(@Description, @IdMuscleGroup, CURRENT_TIMESTAMP)";

            var result = await db.ExecuteAsync(sql, new { category.Description, category.IdMuscleGroup });

            return result > 0;

        }

        public async Task<bool> UpdateCategory(Category category)
        {
            var db = DbConnection();

            var sql = @"UPDATE categoria
                          SET  descripcion = @Description, 
                               ID_grupo_muscular_FK = @IdMuscleGroup, 
                               f_ult_act = CURRENT_TIMESTAMP
                         WHERE ID_categoria = @Id ";

            var result = await db.ExecuteAsync(sql, new { category.Description, category.IdMuscleGroup, Id = category.ID });

            return result > 0;
        }
        public async Task<bool> DeleteCategory(int id)
        {
            var db = DbConnection();

            var sql = @"DELETE FROM categoria
                        WHERE ID_categoria = @Id ";
            var result = await db.ExecuteAsync(sql, new { Id = id });

            return result > 0;
        }
    }
}
