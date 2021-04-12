using System.Collections.Generic;
using System.Threading.Tasks;
using MyUniversity.UserManager.Models.University;

namespace MyUniversity.UserManager.Services
{
    public interface IUniversityService
    {
        Task<UniversityModel> CreateUniversityAsync(CreateUniversityModel model, string accessToken);
        Task<IEnumerable<UniversityModel>> GetAllUniversitiesAsync(string accessToken);
        Task<UniversityModel> UpdateUniversityAsync(UpdateUniversityModel model, string accessToken);
        Task<bool> DeleteUniversityAsync(string id, string accessToken);
    }
}
