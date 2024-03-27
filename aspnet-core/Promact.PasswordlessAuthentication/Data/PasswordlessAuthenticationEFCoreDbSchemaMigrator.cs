using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace Promact.PasswordlessAuthentication.Data;

public class PasswordlessAuthenticationEFCoreDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public PasswordlessAuthenticationEFCoreDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the PasswordlessAuthenticationDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<PasswordlessAuthenticationDbContext>()
            .Database
            .MigrateAsync();
    }
}
