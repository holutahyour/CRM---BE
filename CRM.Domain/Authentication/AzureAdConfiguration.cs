namespace CRM.Domain.Authentication
{
    public class AzureAdConfiguration
    {
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string Audience { get; set; }
        public string Authority { get; set; }
        public string Scope { get; set; }
    }
}
