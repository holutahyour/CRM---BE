using CRM.Base.Common.Domain.Entities;
using System.Linq.Expressions;

namespace CRM.Data
{
    public class ApplicationDbContext : Base.Common.Repositories.ApplicationDbContext
    {
        private readonly DbContextOptions<ApplicationDbContext> options;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            this.options = options;
        }

        //Core
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Ethnicity> Ethnicities { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        //Parameter
        public DbSet<ParameterDefinition> ParameterDefinitions { get; set; }
        public DbSet<ParameterValue> ParameterValues { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Equal(
                        Expression.Property(parameter, nameof(AuditableEntity.IsDeleted)),
                        Expression.Constant(false)
                    );

                    var filter = Expression.Lambda(body, parameter);

                    builder.Entity(entityType.ClrType)
                                .HasQueryFilter(filter);
                }
            }
        }
    }
}
