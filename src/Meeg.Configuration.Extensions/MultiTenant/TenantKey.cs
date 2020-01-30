using System;

namespace Meeg.Configuration.Extensions.MultiTenant
{
    public class TenantKey
    {
        private readonly string key;

        public int Length => key.Length;

        public TenantKey(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            key = value.ToString();

            if (key.Length == 0)
            {
                throw new ArgumentException("Cannot be empty.", nameof(value));
            }
        }

        public static implicit operator string(TenantKey tenantKey)
        {
            return tenantKey.key;
        }

        public static explicit operator TenantKey(string value)
        {
            return new TenantKey(value);
        }

        public override string ToString()
        {
            return key;
        }

        public override bool Equals(object obj)
        {
            return obj is TenantKey other && Equals(key, other.key);
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }
    }
}
