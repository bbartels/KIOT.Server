namespace KIOT.Server.Core.Models.Data
{
    public readonly struct CustomerCode
    {
        public string Code { get; }
        public string ServiceProvider => Code.Substring(0, 4);
        public string CustomerId => Code.Substring(5);

        public CustomerCode(string customerCode)
        {
            Code = customerCode;
        }
    }
}
