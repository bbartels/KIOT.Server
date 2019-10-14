using System;

namespace KIOT.Server.Core.Services
{
    public partial class ApiKeyService : IApiKeyService
    {
        public string ApiKey { get; } = Environment.GetEnvironmentVariable("ExternalApiKey");
    }
}
