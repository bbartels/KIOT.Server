using System;
using System.Threading.Tasks;

using KIOT.Server.Core.Data.Response.Caretaker;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Core.Data.Persistence.Application
{
    public interface ICaretakerForCustomerRequestRepository : IRepository<CaretakerForCustomerRequest>
    {
        Task<GetPendingCaretakerRequestsResponse> GetRequests(string username);
        Task<DataResponse<CaretakerForCustomerRequest>> CheckRequestExistsAndDelete(Guid requestId, string username);
    }
}
