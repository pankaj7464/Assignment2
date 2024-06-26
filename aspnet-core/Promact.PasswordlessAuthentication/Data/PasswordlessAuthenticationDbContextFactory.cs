using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Promact.PasswordlessAuthentication.Data;

public class PasswordlessAuthenticationDbContextFactory : IDesignTimeDbContextFactory<PasswordlessAuthenticationDbContext>
{
    public PasswordlessAuthenticationDbContext CreateDbContext(string[] args)
    {

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<PasswordlessAuthenticationDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new PasswordlessAuthenticationDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
