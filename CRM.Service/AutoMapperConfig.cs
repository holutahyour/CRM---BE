namespace CRM.Service
{
    public class AutoMapperConfig : MapperConfig
    {
        public AutoMapperConfig()
        {
        }

        protected override void ConfigureCustomMappings()
        {
            //Core 
            CreateMap<Country, CountryResponse>().ReverseMap();
            CreateMap<UpdateCountryRequest, Country>().ReverseMap();
            CreateMap<CreateCountryRequest, Country>().ReverseMap();
            CreateMap<City, CityResponse>().ReverseMap();
            CreateMap<UpdateCityRequest, City>().ReverseMap();
            CreateMap<CreateCityRequest, City>().ReverseMap();
            CreateMap<Gender, GenderResponse>().ReverseMap();
            CreateMap<UpdateGenderRequest, Gender>().ReverseMap();
            CreateMap<CreateGenderRequest, Gender>().ReverseMap();
            CreateMap<State, StateResponse>().ReverseMap();
            CreateMap<UpdateStateRequest, State>().ReverseMap();
            CreateMap<CreateStateRequest, State>().ReverseMap();
            CreateMap<Ethnicity, EthnicityResponse>().ReverseMap();
            CreateMap<UpdateEthnicityRequest, Ethnicity>().ReverseMap();
            CreateMap<CreateEthnicityRequest, Ethnicity>().ReverseMap();

            //Core/Access Control
            CreateMap<Tenant, TenantDTO>().ReverseMap();
            CreateMap<Menu, MenuDTO>()
                .ForCtorParam("PermissionIds",
                    opt => opt.MapFrom(src => src.MenuPermissions
                        .Select(mp => mp.PermissionId)
                        .ToList()))
                .ForCtorParam("Children",
                    opt => opt.MapFrom(src => src.Children))
                .ReverseMap();

            CreateMap<CreateMenuRequest, Menu>().ReverseMap();
            CreateMap<UpdateMenuRequest, Menu>().ReverseMap();

            // User -> UserDTO: map UserRoles navigation to List<string> of role names
            CreateMap<User, UserDTO>()
                .ForCtorParam("Roles",
                    opt => opt.MapFrom(src => src.UserRoles
                        .Where(ur => ur.Role != null)
                        .Select(ur => ur.Role.Name)
                        .ToList()));
            // Reverse: UserDTO -> User ignores the computed Roles list
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

            CreateMap<UpdateUserRequest, User>().ReverseMap();
            CreateMap<CreateUserRequest, User>().ReverseMap();

            // Role -> RoleDTO: map RolePermissions navigation to List<string> of permission names
            CreateMap<Role, RoleDTO>()
                .ForCtorParam("Permissions",
                    opt => opt.MapFrom(src => src.RolePermissions
                        .Where(rp => rp.Permission != null)
                        .Select(rp => rp.Permission.Name)
                        .ToList()))
                .ForCtorParam("PermissionIds",
                    opt => opt.MapFrom(src => src.RolePermissions
                        .Select(rp => rp.PermissionId)
                        .ToList()));
            // Reverse: RoleDTO -> Role ignores the computed Permissions list
            CreateMap<RoleDTO, Role>()
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());

            CreateMap<UpdateRoleRequest, Role>().ReverseMap();
            CreateMap<CreateRoleRequest, Role>().ReverseMap();

            //finance and revenue
            CreateMap<ParameterValue, ParameterValueResponse>().ReverseMap();
            CreateMap<UpdateParameterValueRequest, ParameterValue>().ReverseMap();
            CreateMap<CreateParameterValueRequest, ParameterValue>().ReverseMap();
            CreateMap<Permission, PermissionDTO>().ReverseMap();

            // Inventory
            CreateMap<Item, ItemResponse>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location != null ? src.Location.Name : null))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Vendor != null ? src.Vendor.Name : null))
                .ReverseMap();
            CreateMap<CreateItemRequest, Item>().ReverseMap();
            CreateMap<UpdateItemRequest, Item>().ReverseMap();

            CreateMap<Category, CategoryResponse>().ReverseMap();
            CreateMap<CreateCategoryRequest, Category>().ReverseMap();
            CreateMap<UpdateCategoryRequest, Category>().ReverseMap();

            CreateMap<Location, LocationResponse>().ReverseMap();
            CreateMap<CreateLocationRequest, Location>().ReverseMap();
            CreateMap<UpdateLocationRequest, Location>().ReverseMap();

            CreateMap<Batch, BatchResponse>().ReverseMap();
            CreateMap<CreateBatchRequest, Batch>().ReverseMap();
            CreateMap<UpdateBatchRequest, Batch>().ReverseMap();

            CreateMap<ItemLocation, ItemLocationResponse>().ReverseMap();
            CreateMap<CreateItemLocationRequest, ItemLocation>().ReverseMap();
            CreateMap<UpdateItemLocationRequest, ItemLocation>().ReverseMap();

            CreateMap<InventoryTransaction, InventoryTransactionResponse>().ReverseMap();
            CreateMap<CreateInventoryTransactionRequest, InventoryTransaction>().ReverseMap();
            CreateMap<UpdateInventoryTransactionRequest, InventoryTransaction>().ReverseMap();

            // Organization
            CreateMap<Department, DepartmentResponse>().ReverseMap();
            CreateMap<CreateDepartmentRequest, Department>().ReverseMap();
            CreateMap<UpdateDepartmentRequest, Department>().ReverseMap();

            // Requisitions
            CreateMap<Requisition, RequisitionResponse>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
                .ForMember(dest => dest.SubmittedByName, opt => opt.MapFrom(src => src.SubmittedByUser != null ? src.SubmittedByUser.FullName : null))
                .ForMember(dest => dest.ActionedByName, opt => opt.MapFrom(src => src.ActionedByUser != null ? src.ActionedByUser.FullName : null))
                .ReverseMap();
            CreateMap<CreateRequisitionRequest, Requisition>().ReverseMap();
            CreateMap<UpdateRequisitionRequest, Requisition>().ReverseMap();

            CreateMap<Activity, ActivityResponse>().ReverseMap();
            CreateMap<CreateActivityRequest, Activity>().ReverseMap();
            CreateMap<UpdateActivityRequest, Activity>().ReverseMap();

            CreateMap<Incident, IncidentResponse>().ReverseMap();
            CreateMap<CreateIncidentRequest, Incident>().ReverseMap();
            CreateMap<UpdateIncidentRequest, Incident>().ReverseMap();

            CreateMap<ItemRequest, ItemRequestResponse>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.SubmittedByUser != null ? src.SubmittedByUser.FullName : null))
                .ForMember(dest => dest.ActionedByName, opt => opt.MapFrom(src => src.ActionedByUser != null ? src.ActionedByUser.FullName : null))
                .ReverseMap();
            CreateMap<CreateItemRequestRequest, ItemRequest>().ReverseMap();
            CreateMap<UpdateItemRequestRequest, ItemRequest>().ReverseMap();

            CreateMap<MonthlyReport, MonthlyReportResponse>().ReverseMap();
            CreateMap<CreateMonthlyReportRequest, MonthlyReport>().ReverseMap();
            CreateMap<UpdateMonthlyReportRequest, MonthlyReport>().ReverseMap();

            // Orders
            CreateMap<Vendor, VendorResponse>().ReverseMap();
            CreateMap<CreateVendorRequest, Vendor>().ReverseMap();
            CreateMap<UpdateVendorRequest, Vendor>().ReverseMap();
        }
    }
}
