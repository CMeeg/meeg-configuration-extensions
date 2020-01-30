using System.Collections.Generic;
using System.Linq;

namespace Meeg.Configuration.Extensions.MultiTenant
{
    public static class AppConfigurationExtensions
    {
        public static string GetValue(this IAppConfigurationRoot appConfig, string key, TenantKey tenant)
        {
            return appConfig.GetValue<string>(key, tenant);
        }

        public static string GetValue(this IAppConfigurationRoot appConfig, string key, TenantKey tenant, string defaultValue)
        {
            return appConfig.GetValue<string>(key, tenant, defaultValue);
        }

        public static IAppConfigurationSection GetSection(this IAppConfigurationRoot appConfig, string key, TenantKey tenant)
        {
            if (string.IsNullOrEmpty(tenant))
            {
                // Return the "global" section 

                return appConfig.GetSection(key);
            }

            return new TenantAppConfigurationSection(appConfig, key, tenant);
        }

        public static IEnumerable<IAppConfigurationSection> GetChildren(this IAppConfigurationRoot appConfig, TenantKey tenant)
        {
            if (string.IsNullOrEmpty(tenant))
            {
                return Enumerable.Empty<IAppConfigurationSection>();
            }

            IAppConfigurationSection section = new TenantAppConfigurationSection(appConfig, tenant);

            return section.GetChildren();
        }
    }
}
