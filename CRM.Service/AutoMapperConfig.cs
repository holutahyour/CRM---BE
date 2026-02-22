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
            CreateMap<Country, CountryResponse>();
            CreateMap<UpdateCountryRequest, Country>();
            CreateMap<CreateCountryRequest, Country>();
            CreateMap<City, CityResponse>();
            CreateMap<UpdateCityRequest, City>();
            CreateMap<CreateCityRequest, City>();
            CreateMap<Gender, GenderResponse>();
            CreateMap<UpdateGenderRequest, Gender>();
            CreateMap<CreateGenderRequest, Gender>();
            CreateMap<State, StateResponse>();
            CreateMap<UpdateStateRequest, State>();
            CreateMap<CreateStateRequest, State>();
            CreateMap<Ethnicity, EthnicityResponse>();
            CreateMap<UpdateEthnicityRequest, Ethnicity>();
            CreateMap<CreateEthnicityRequest, Ethnicity>();

            //Core/Access Control
            CreateMap<MenuDTO, Menu>();

            CreateMap<UserDTO, User>();
            CreateMap<UpdateUserRequest, User>();
            CreateMap<CreateUserRequest, User>();

            CreateMap<RoleDTO, Role>();
            CreateMap<UpdateRoleRequest, Role>();
            CreateMap<CreateRoleRequest, Role>();

            //finance and revenue
            CreateMap<ParameterValue, ParameterValueResponse>();
            CreateMap<UpdateParameterValueRequest, ParameterValue>();
            CreateMap<CreateParameterValueRequest, ParameterValue>();


        }
    }
}
