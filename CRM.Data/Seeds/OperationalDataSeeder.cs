namespace CRM.Data.Seeds;

public class OperationalDataSeeder
{
    private readonly ApplicationDbContext _context;

    public OperationalDataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task InitializeAsync()
    {
        // Operational Seed Data

        //if (!await _context.FormTemplates.AnyAsync())
        //{
        //    var formTemplates = FormTemplateSeedData.GenerateFormTemplateData();
        //    await _context.FormTemplates.AddRangeAsync(formTemplates);
        //}



        await _context.SaveChangesAsync();
    }

}
