namespace CRM.Domain.Enums;

/// <summary>
/// Production status for order requests and production batches — the "STATUS" list on the
/// "Dropdown keys" sheet of the Batch Production Scheduling workbook, in the same order.
/// </summary>
public enum ProcessingStatus
{
    NotStarted = 1,
    InProgress = 2,
    Complete = 3,
    OnHold = 4,
    Overdue = 5,
    NeedsReview = 6,
    NeedsUpdate = 7
}
