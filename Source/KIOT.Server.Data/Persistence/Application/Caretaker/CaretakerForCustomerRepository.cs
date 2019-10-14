using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;

namespace KIOT.Server.Data.Persistence.Application.Caretaker
{
    internal class CaretakerForCustomerRepository : Repository<CaretakerForCustomer>, ICaretakerForCustomerRepository
    {
        public CaretakerForCustomerRepository(KIOTContext context) : base(context) { }
    }
}
