namespace CRM.Base.Repositories.Implementations
{
    public class MSSQLRepository<T, I> : MSSQLBaseRepository<T, I>
    where T : BaseEntity<I>
    {
        public MSSQLRepository(IApplicationDbContext context) : base(context)
        {
        }
    }
}
