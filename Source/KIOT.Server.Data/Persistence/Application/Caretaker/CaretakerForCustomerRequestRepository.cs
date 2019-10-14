using System;
using System.Threading.Tasks;

using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Data.Response.Caretaker;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Data.Persistence.Application.Caretaker
{
    internal class CaretakerForCustomerRequestRepository : Repository<CaretakerForCustomerRequest>, ICaretakerForCustomerRequestRepository
    {
        private KIOTContext KIOTContext => Context as KIOTContext;

        private readonly IUnitOfWork<ICustomerRepository, Customer> _customerUnitOfWork;
        private readonly IUnitOfWork<ICaretakerRepository, Core.Models.Application.Caretaker> _caretakerUnitOfWork;

        public CaretakerForCustomerRequestRepository(KIOTContext context, IUnitOfWork<ICustomerRepository, Customer> customerUnitOfWork,
                    IUnitOfWork<ICaretakerRepository, Core.Models.Application.Caretaker> caretakerUnitOfWork) :
            base(context)
        {
            _customerUnitOfWork = customerUnitOfWork;
            _caretakerUnitOfWork = caretakerUnitOfWork;
        }

        public async Task<GetPendingCaretakerRequestsResponse> GetRequests(string username)
        {
            var id = await _caretakerUnitOfWork.Repository.GetIdAsync(x => x.Username == username);

            return id is int caretakerId
                ? new GetPendingCaretakerRequestsResponse(
                    await GetAsync(x => !x.Handled && x.CaretakerId == caretakerId, null, $"{nameof(Customer)}"))
                : new GetPendingCaretakerRequestsResponse(null,
                    new Error("CaretakerNotFound", $"Could not find caretaker with username: '{username}'"));
        }

        public async Task<DataResponse<CaretakerForCustomerRequest>> CheckRequestExistsAndDelete(Guid requestId, string username)
        {
            var request = await SingleOrDefaultAsync(x => x.Guid == requestId && x.Caretaker.Username == username,
                $"{nameof(Core.Models.Application.Caretaker)}");

            if (request is null)
            {
                return new DataResponse<CaretakerForCustomerRequest>(null,
                    new Error("InvalidRequest", "Request does not exist."));
            }

            Remove(request);
            await Context.SaveChangesAsync();

            return request.Handled
                ? new DataResponse<CaretakerForCustomerRequest>(null, new Error("InvalidRequest", "Request has already been handled."))
                : new DataResponse<CaretakerForCustomerRequest>(request);
        }
    }
}
