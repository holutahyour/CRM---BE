using CRM.Data.Seeds.Data.Core;

namespace CRM.Data.Seeds;

public class Seeder
{
    private readonly CoreDbContext _context;

    public Seeder(CoreDbContext context)
    {
        _context = context;
    }
    public void Intialize()
    {
        //Core            

        var genders = GenderSeedData.GenerateGenderData();

        if (!_context.Genders.Any())
        {
            _context.Genders.AddRange(genders);
        }

        var countries = CountrySeedData.GenerateCountryData();

        if (!_context.Countries.Any())
        {
            var uniqueCountries = countries
                .GroupBy(c => c.CountryCode)
                .Select(g => g.First())
                .ToList();

            foreach (var country in uniqueCountries)
                country.Code = $"COUNTRY-{country.CountryCode}";

            _context.Countries.AddRange(uniqueCountries);
            _context.SaveChanges();
        }

        var states = StateSeedData.GenerateStateData();

        if (!_context.States.Any())
        {
            var uniqueStates = states
                .GroupBy(c => c.Abbreviation)
                .Select(g => g.First())
                .ToList();

            foreach (var state in uniqueStates)
                state.Code = $"STATE-{state.Abbreviation}";

            _context.States.AddRange(uniqueStates);
            _context.SaveChanges();
        }

        var ethnicities = EthnicitySeedData.GenerateEthnicityData();

        if (!_context.Ethnicities.Any())
        {
            _context.Ethnicities.AddRange(ethnicities);
        }

        var parameterDefinitions = ParameterDefinitionSeedData.GenerateParameterDefinitionData();

        if (!_context.ParameterDefinitions.Any())
        {
            _context.ParameterDefinitions.AddRange(parameterDefinitions);
        }

        _context.SaveChanges();
    }
}
