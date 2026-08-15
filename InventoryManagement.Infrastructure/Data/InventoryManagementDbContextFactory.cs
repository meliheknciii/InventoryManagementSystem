using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InventoryManagement.Infrastructure.Data;

/// <summary>
/// Enables 'dotnet ef' design-time tooling (migrations) to construct the DbContext
/// without requiring the API project to be the EF Core tooling startup project.
/// </summary>
public class InventoryManagementDbContextFactory : IDesignTimeDbContextFactory<InventoryManagementDbContext>
{
    public InventoryManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryManagementDbContext>();

        // Design-time only connection string, used solely to scaffold/apply migrations.
        // The real connection string is supplied at runtime via appsettings.json / DI.
        optionsBuilder.UseSqlServer(
            "Server=DESKTOP-0VNHJA3;Database=InventoryManagementDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");

        return new InventoryManagementDbContext(optionsBuilder.Options);
    }
}
