using CRM.Base.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities;

/// <summary>
/// A finished product the processing unit makes — the "PRODUCT IDENTIFICATION" sheet merged with
/// "PRODUCT DURATION". Codes are kept as the free text the team uses (e.g.
/// <c>SFL/002/GIG2/A10/0525/01</c>); nothing parses them.
/// </summary>
[Table("ops_products")]
public class ProcessingProduct : TenantEntity<Guid>
{
    public string Name { get; set; } = "";
    public string? ProductCode { get; set; }
    public string? Upc { get; set; }
    public string? Sku { get; set; }
    public string? RawMaterialId { get; set; }

    /// <summary>Free text, e.g. "12Hrs".</summary>
    public string? ProcessingDuration { get; set; }
}
