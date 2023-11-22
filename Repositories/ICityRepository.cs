using ApiMySQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMySQL.Data.Repositories
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllCities();
        Task<City> GetDetails(int id); 
        Task<bool> InsertCity(City city);
        Task<bool> UpdateCity(City city);         
        //recuerda que en el ejemplo recibe un objeto City, no un ID
        Task<bool> DeleteCity(int id);
    }
}
