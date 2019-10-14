using System.Threading.Tasks;

using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Data.Persistence.Application.Caretaker
{
    internal class CaretakerRepository : Repository<Core.Models.Application.Caretaker>, ICaretakerRepository
    {
        private KIOTContext KIOTContext => Context as KIOTContext;

        public CaretakerRepository(KIOTContext context) : base(context) { }

        public async Task<DataResponse<Core.Models.Application.Caretaker>> GetByUsername(string username)
        {
            var customer = await SingleOrDefaultAsync(c => c.Username == username);

            return customer != null
                ? new DataResponse<Core.Models.Application.Caretaker>(customer)
                : new DataResponse<Core.Models.Application.Caretaker>(null,
                    new Error("InvalidUsername", $"Could not find Customer with username: '{username}'"));
        }
    }
}
