using System;
using System.Collections.Generic;
using System.Linq;

namespace Meeg.Configuration.Extensions.MultiTenant
{
    public class TenantAppConfigurationSection : IAppConfigurationSection
    {
        private readonly IAppConfigurationRoot root;
        private readonly TenantKey tenant;
        private readonly IAppConfigurationSection tenantSection;
        private readonly IAppConfigurationSection globalSection;

        private string sectionKey;
        public string Key => sectionKey ?? (sectionKey = AppConfigurationPath.GetSectionKey(Path));
        public string Value { get; }
        public string Path { get; }

        public string this[string key] => GetValue(key);

        public TenantAppConfigurationSection(IAppConfigurationRoot root, TenantKey tenant)
            : this(root, tenant, tenant)
        {
        }

        public TenantAppConfigurationSection(IAppConfigurationRoot root, string key, TenantKey tenant)
        {
            this.root = root;
            this.tenant = tenant;

            string tenantKey, globalKey;

            if (tenant == key)
            {
                tenantKey = tenant;
                globalKey = null;
            }
            else if (key.StartsWith($"{tenant}{AppConfigurationPath.KeyDelimiter}", StringComparison.OrdinalIgnoreCase))
            {
                tenantKey = key;
                globalKey = key.Substring(tenant.Length + 1);
            }
            else
            {
                tenantKey = AppConfigurationPath.Combine(tenant, key);
                globalKey = key;
            }

            tenantSection = root.GetSection(tenantKey);
            globalSection = globalKey == null ? null : root.GetSection(globalKey);

            Path = tenantKey;
            Value = tenantSection.Exists() ? tenantSection.Value : globalSection?.Value;
        }

        private string GetValue(string key)
        {
            IAppConfigurationSection tenantKeySection = tenantSection
                .GetChildren()
                .FirstOrDefault(section => section.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (tenantKeySection != null)
            {
                return tenantKeySection.Value;
            }

            return globalSection?[key];
        }

        public IAppConfigurationSection GetSection(string key)
        {
            return new TenantAppConfigurationSection(
                root,
                AppConfigurationPath.Combine(Path, key),
                tenant
            );
        }

        public IEnumerable<IAppConfigurationSection> GetChildren()
        {
            // We will loop through all of the global children and see if there is a tenant equivalent

            var children = new List<IAppConfigurationSection>();

            var globalChildren = globalSection == null
                ? root.GetChildren()
                : globalSection.GetChildren();

            foreach (IAppConfigurationSection globalChild in globalChildren)
            {
                if (globalChild.Path.Equals(tenant, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string tenantKey = AppConfigurationPath.Combine(tenant, globalChild.Path).ToLower();

                // Tenant settings take precedence over global settings

                if (root.AllKeys.Contains(tenantKey))
                {
                    var tenantChild = root.GetSection(tenantKey);

                    children.Add(tenantChild);
                }
                else
                {
                    var tenantChild = new TenantAppConfigurationSection(
                        root,
                        AppConfigurationPath.Combine(tenant, globalChild.Path),
                        tenant
                    );

                    children.Add(tenantChild);
                }
            }

            return children.AsReadOnly();
        }
    }
}
