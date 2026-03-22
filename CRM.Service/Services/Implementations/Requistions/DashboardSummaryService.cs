using CRM.Base.Common.Domain.Common;
using CRM.Base.Common.Domain.Entities;
using CRM.Base.Common.Repositories;
using CRM.Base.Common.Repositories.Interfaces;
using CRM.Base.Common.Services.Implementation;
using CRM.Domain.DTOs;
using CRM.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRM.Services.Implementations;

public class DashboardSummaryService : MSSQLBaseService<DashboardSummary, Guid>, IDashboardSummaryService
{
    private readonly IMSSQLRepository<Requisition, Guid> _requisitionRepository;
    private readonly IMSSQLRepository<ItemRequest, Guid> _itemRequestRepository;
    private readonly IMSSQLRepository<Incident, Guid> _incidentRepository;
    private readonly IMSSQLRepository<AuditLog, long> _auditLogRepository;

    public DashboardSummaryService(
    IMSSQLRepository<DashboardSummary, Guid> baseRepository,
    IMSSQLRepository<Requisition, Guid> requisitionRepository,
    IMSSQLRepository<ItemRequest, Guid> itemRequestRepository,
    IMSSQLRepository<Incident, Guid> incidentRepository,
    IMSSQLRepository<AuditLog, long> auditLogRepository,
    IApplicationDbContext context,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor
    )
        : base(baseRepository, auditLogRepository, context, mapper, httpContextAccessor)
    {
        _requisitionRepository = requisitionRepository;
        _itemRequestRepository = itemRequestRepository;
        _incidentRepository = incidentRepository;
        _auditLogRepository = auditLogRepository;
    }

    public override async Task<Result<dynamic>> GetAllAsync<TResponse>(
        string? search = null,
        string? filter = null,
        int page = 1,
        int pageSize = 10,
        string? select = null,
        string? orderBy = null,
        CRM.Base.Enums.OrderDirectionEnum orderDirection = CRM.Base.Enums.OrderDirectionEnum.Asc,
        string baseUrl = "{app_url}")
    {
        Result<dynamic> result = new(false);

        var pendingRequisitions = await (await _requisitionRepository.GetAllAsync()).CountAsync(r => r.Status == RequisitionStatus.Pending);
        var approvedRequisitions = await (await _requisitionRepository.GetAllAsync()).CountAsync(r => r.Status == RequisitionStatus.Approved);
        var itemRequests = await (await _itemRequestRepository.GetAllAsync()).CountAsync(r => r.Status == ItemRequestStatus.Pending); // Assuming pending is default
        var openIncidents = await (await _incidentRepository.GetAllAsync()).CountAsync(i => i.Status == IncidentStatus.Open);

        var summary = new DashboardSummaryResponse
        {
            PendingRequisitions = pendingRequisitions,
            ApprovedRequisitions = approvedRequisitions,
            ItemRequests = itemRequests,
            LowStock = 5, // Mocked for now
            OpenIncidents = openIncidents,
            MonthlyGoalsAchieved = 85m // Mocked KPI
        };

        var responseData = new List<DashboardSummaryResponse> { summary };

        result.SetMeta(1, 1, 1, 1, 1, null, null, null, null, null);
        result.SetSuccess(responseData, "Retrieved Successfully.");

        return result;
    }
}