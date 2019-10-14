using System.Threading.Tasks;

namespace KIOT.Server.Business.Abstractions.Jobs
{
    internal interface IBackgroundJob<in TRequest> where TRequest : IJobRequest
    {
        Task Run(TRequest request);
    }
}
