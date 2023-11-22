using ApiMySQL.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApiMySQL.Data.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly MySQLConfiguration _connectionString;

        public CityRepository(MySQLConfiguration connectionString)
        {
            _connectionString = connectionString;
        }

        protected MySqlConnection DbConnection()
        {
            return new MySqlConnection(_connectionString.ConnectionString);
        }

        public async Task<bool> DeleteCity(int id)
        {
            var db = DbConnection();

            var sql = @"DELETE FROM city
                        WHERE ID = @Id ";
            var result = await db.ExecuteAsync(sql, new {Id= id});
            
            return result > 0;
        }

        public async Task<IEnumerable<City>> GetAllCities()
        {
            var db = DbConnection();

            var sql = @"SELECT ID, Name, CountryCode, District, Population
                        FROM city";

            return await db.QueryAsync<City>(sql, new { });
        }

        public async Task<City> GetDetails(int id)
        {
            var db = DbConnection();

            var sql = @"SELECT ID, Name, CountryCode, District, Population
                        FROM city
                        WHERE ID = @Id ";

            return await db.QueryFirstOrDefaultAsync<City>(sql, new { Id = id });
        }

        public async Task<bool> InsertCity(City city)
        {
            var db = DbConnection();

            var sql = @"INSERT INTO city(Name, CountryCode, District, Population)
                       VALUES(@Name, @CountryCode, @District, @Population)";

            var result = await db.ExecuteAsync(sql, new { city.Name, city.CountryCode, city.District, city.Population });

            return result > 0;

        }

        public async Task<bool> UpdateCity(City city)
        {
            var db = DbConnection();

            var sql = @"UPDATE city
                        SET Name = @Name, 
                            CountryCode = @CountryCode, 
                            District = @District,
                            Population @Population)
                       WHERE ID = @Id ";

            var result = await db.ExecuteAsync(sql, new { city.Name, city.CountryCode, city.District, city.Population });

            return result > 0;
        }
    }
}
