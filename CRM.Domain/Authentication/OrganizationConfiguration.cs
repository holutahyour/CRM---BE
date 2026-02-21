namespace CRM.Domain.Authentication
{
    public class OrganizationConfiguration
    {
        public Guid id { get; set; }
        public string DisplayName { get; set; }
        public string AppName { get; set; }
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string Audience { get; set; }
        public string Authority { get; set; }
        public string Scope { get; set; }
        public string DatabaseServer { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseUserId { get; set; }
        public string DatabasePassword { get; set; }
    }
}
