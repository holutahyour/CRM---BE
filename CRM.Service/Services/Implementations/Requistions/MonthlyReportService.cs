namespace CRM.Services.Implementations;

public class MonthlyReportService : MSSQLBaseService<MonthlyReport, Guid>, IMonthlyReportService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MonthlyReportService(
    IMSSQLRepository<MonthlyReport, Guid> baseRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        if (request is CRM.Domain.DTOs.CreateMonthlyReportRequest createReq)
        {
            var user = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;
            if (user != null)
            {
                createReq.SubmittedBy = user.Id;
            }
        }

        return await base.CreateAsync<TResponse, TRequest>(request);
    }
}