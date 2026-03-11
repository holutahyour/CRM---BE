namespace CRM.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        //AzureBlobService
        services.AddSingleton<AzureBlobService>();

        //Core
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IStateService, StateService>();
        services.AddScoped<IGenderService, GenderService>();
        services.AddScoped<IEthnicityService, EthnicityService>();
        services.AddScoped<IParameterValueService, ParameterValueService>();

        //Core/Access Control
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPermissionService, PermissionService>();

        //Organization
        services.AddScoped<IDepartmentService, DepartmentService>();

        //Inventory
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IBatchService, BatchService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IItemLocationService, ItemLocationService>();
        services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();
        services.AddScoped<IItemLocationService, ItemLocationService>();

        //Requistions
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IDashboardSummaryService, DashboardSummaryService>();
        services.AddScoped<IIncidentService, IncidentService>();
        services.AddScoped<IItemRequestService, ItemRequestService>();
        services.AddScoped<IMonthlyReportService, MonthlyReportService>();
        services.AddScoped<IRequisitionService, RequisitionService>();




        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperConfig>();
        });

        // Register IMapper
        var mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        //services.AddAutoMapper(typeof(DependencyInjection));
        services.AddControllersWithViews();

        services.AddHttpClient();





        return services;
    }
}
