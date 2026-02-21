namespace CRM.Domain.Enums;

public enum PurchaseOrderStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Ordered = 4,
    PartiallyReceived = 5,
    Received = 6,
    Cancelled = 7
}

public enum SalesOrderStatus
{
    Draft = 1,
    Confirmed = 2,
    Processing = 3,
    PartiallyFulfilled = 4,
    Fulfilled = 5,
    Shipped = 6,
    Delivered = 7,
    Cancelled = 8
}
