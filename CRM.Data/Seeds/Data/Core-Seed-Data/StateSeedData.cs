namespace CRM.Data.Seeds.Data.Core
{
    public class StateSeedData
    {
        public static State[] GenerateStateData()
        {
            var data = new List<State>
            {
                new State {  Name = "Abia", Abbreviation = "AB", Capital = "Umuahia", Population = "2,845,380", Area = "6,320 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Adamawa", Abbreviation = "AD", Capital = "Yola", Population = "3,178,950", Area = "36,917 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Akwa Ibom", Abbreviation = "AK", Capital = "Uyo", Population = "5,482,177", Area = "7,081 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Anambra", Abbreviation = "AN", Capital = "Awka", Population = "4,177,828", Area = "4,844 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Bauchi", Abbreviation = "BA", Capital = "Bauchi", Population = "4,653,066", Area = "45,837 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Bayelsa", Abbreviation = "BY", Capital = "Yenagoa", Population = "1,704,515", Area = "10,773 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Benue", Abbreviation = "BE", Capital = "Makurdi", Population = "4,253,641", Area = "34,059 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Borno", Abbreviation = "BO", Capital = "Maiduguri", Population = "4,171,104", Area = "70,898 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Cross River", Abbreviation = "CR", Capital = "Calabar", Population = "2,892,988", Area = "20,156 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Delta", Abbreviation = "DE", Capital = "Asaba", Population = "4,112,445", Area = "17,698 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Ebonyi", Abbreviation = "EB", Capital = "Abakaliki", Population = "2,176,947", Area = "5,670 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Edo", Abbreviation = "ED", Capital = "Benin City", Population = "3,233,366", Area = "17,802 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Ekiti", Abbreviation = "EK", Capital = "Ado Ekiti", Population = "2,398,957", Area = "6,353 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Enugu", Abbreviation = "EN", Capital = "Enugu", Population = "3,267,837", Area = "7,161 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Federal Capital Territory", Abbreviation = "FC", Capital = "Abuja", Population = "1,406,239", Area = "7,315 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Gombe", Abbreviation = "GO", Capital = "Gombe", Population = "2,365,040", Area = "18,768 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Imo", Abbreviation = "IM", Capital = "Owerri", Population = "3,927,563", Area = "5,530 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Jigawa", Abbreviation = "JI", Capital = "Dutse", Population = "4,361,002", Area = "23,154 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Kaduna", Abbreviation = "KD", Capital = "Kaduna", Population = "6,113,503", Area = "46,053 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Kano", Abbreviation = "KN", Capital = "Kano", Population = "9,401,288", Area = "20,131 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Katsina", Abbreviation = "KT", Capital = "Katsina", Population = "5,801,584", Area = "24,192 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Kebbi", Abbreviation = "KE", Capital = "Birnin Kebbi", Population = "3,256,541", Area = "36,800 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Kogi", Abbreviation = "KO", Capital = "Lokoja", Population = "3,314,043", Area = "29,833 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Kwara", Abbreviation = "KW", Capital = "Ilorin", Population = "2,365,353", Area = "36,825 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Lagos", Abbreviation = "LA", Capital = "Ikeja", Population = "9,113,605", Area = "3,577 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Nasarawa", Abbreviation = "NA", Capital = "Lafia", Population = "1,869,377", Area = "27,117 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Niger", Abbreviation = "NI", Capital = "Minna", Population = "3,954,772", Area = "76,363 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Ogun", Abbreviation = "OG", Capital = "Abeokuta", Population = "3,751,140", Area = "16,980 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Ondo", Abbreviation = "ON", Capital = "Akure", Population = "3,460,877", Area = "15,500 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Osun", Abbreviation = "OS", Capital = "Osogbo", Population = "3,416,959", Area = "9,251 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Oyo", Abbreviation = "OY", Capital = "Ibadan", Population = "5,580,894", Area = "28,454 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Plateau", Abbreviation = "PL", Capital = "Jos", Population = "3,206,531", Area = "30,913 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Rivers", Abbreviation = "RI", Capital = "Port Harcourt", Population = "5,198,605", Area = "11,077 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Sokoto", Abbreviation = "SO", Capital = "Sokoto", Population = "3,702,676", Area = "25,973 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Taraba", Abbreviation = "TA", Capital = "Jalingo", Population = "2,294,800", Area = "54,473 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Yobe", Abbreviation = "YO", Capital = "Damaturu", Population = "2,321,339", Area = "45,502 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow },
                new State {  Name = "Zamfara", Abbreviation = "ZA", Capital = "Gusau", Population = "3,278,873", Area = "39,762 km²", CountryCode = "COUNTRY-NG", CreatedBy = "SYSTEM", CreatedOn = DateTime.UtcNow, LastModifiedBy = "SYSTEM", LastModifiedOn = DateTime.UtcNow }

            };

            return [.. data];

        }
    }

}
