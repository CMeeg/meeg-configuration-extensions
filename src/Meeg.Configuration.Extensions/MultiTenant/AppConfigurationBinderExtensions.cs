using System;
using System.Linq;

namespace Meeg.Configuration.Extensions.MultiTenant
{
    public static class AppConfigurationBinderExtensions
    {
        public static T GetValue<T>(this IAppConfigurationRoot configuration, string key, string tenant)
        {
            return configuration.GetValue(key, tenant, default(T));
        }

        public static T GetValue<T>(this IAppConfigurationRoot configuration, string key, string tenant, T defaultValue)
        {
            if (string.IsNullOrEmpty(tenant))
            {
                // Return the "global" setting

                return configuration.GetValue(key, defaultValue);
            }

            // Try to get the tenant setting

            string tenantKey = AppConfigurationPath.Combine(tenant, key);

            if (configuration.AllKeys.Contains(tenantKey))
            {
                return configuration.GetValue(tenantKey, defaultValue);
            }

            // Default to "global" setting

            return configuration.GetValue(key, defaultValue);
        }

        public static T Get<T>(this IAppConfigurationRoot configuration, string tenant)
        {
            return configuration.Get<T>(tenant, _ => { });
        }

        public static T Get<T>(this IAppConfigurationRoot configuration, string tenant,
            Action<AppConfigurationBinderOptions> configureOptions)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var result = configuration.Get(typeof(T), tenant, configureOptions);

            if (result == null)
            {
                return default;
            }

            return (T)result;
        }

        public static object Get(this IAppConfigurationRoot configuration, Type type, string tenant)
        {
            return configuration.Get(type, tenant, _ => { });
        }

        public static object Get(this IAppConfigurationRoot configuration, Type type, string tenant,
            Action<AppConfigurationBinderOptions> configureOptions)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = new TenantAppConfigurationSection(configuration, tenant);

            return section.Get(type, configureOptions);
        }

        public static void Bind(this IAppConfigurationRoot configuration, string key, string tenant, object instance)
        {
            configuration.GetSection(key, tenant).Bind(instance);
        }

        public static void Bind(this IAppConfigurationRoot configuration, string tenant, object instance)
        {
            configuration.Bind(tenant, instance, o => { });
        }

        public static void Bind(this IAppConfigurationRoot configuration, string tenant, object instance, Action<AppConfigurationBinderOptions> configureOptions)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = new TenantAppConfigurationSection(configuration, tenant);

            section.Bind(instance, configureOptions);
        }
    }
}
