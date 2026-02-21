namespace CRM.Core.Domain.Enums
{
    public enum WorkflowStateMode
    {
        Start = 1,
        Normal = 2,
        End = 3
    }

    public enum WorkflowModificationMode
    {
        ResourceOwner = 1,
        StateOwner = 2,
        NextStateOwner = 3,
        Any = 4,
        None = 5
    }

    public enum WorkflowCancelMode
    {
        IsCancelState = 1,
        AllowCancel = 2,
        None = 3
    }

    public enum WorkflowActionMode
    {
        ResourceOwner = 1,
        StateOwner = 2,
        Any = 3,
        None = 4
    }
}
