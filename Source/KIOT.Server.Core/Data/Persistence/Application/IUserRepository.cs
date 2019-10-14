using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Core.Data.Persistence.Application
{
    public interface IUserRepository : IRepository<User>
    {
        Task<DataResponse> RegisterCustomer(string firstName, string lastName, string username, string email,
            string password, string customerId, string phoneNumber);
        Task<DataResponse> RegisterCaretaker(string firstName, string lastName, string username, string email,
            string password, string phoneNumber);
        Task<DataResponse<User>> GetUserByName(string username);
        Task<DataResponse> CheckCredentials(User user, string password);
        Task<DataResponse<IEnumerable<Claim>>> GetClaimsAsync(User user);
    }
}
