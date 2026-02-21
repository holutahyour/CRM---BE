namespace CRM.Base.Common.Domain.Common
{
    public class Error
    {
        public int Code { get; set; }
        public string? Type { get; set; }
        public string? Message { get; set; }
    }
}
