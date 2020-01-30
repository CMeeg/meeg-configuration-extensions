using Meeg.Configuration.Extensions.MultiTenant;
using NUnit.Framework;

namespace Meeg.Configuration.Extensions.Tests.MultiTenant
{
    [TestFixture]
    public class AppConfigurationBinderExtensionsTests
    {
        public class TestSettings
        {
            public string Key1 { get; set; }
            public int Key2 { get; set; }
        }

        [Test]
        public void Get_WithTenantSetting_ReturnsTenantValue()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Key1", "Value1")
                .WithAppSetting("Key2", 2)
                .WithAppSetting("Tenant:Key2", 3)
                .Build();

            var config = new AppConfiguration(configManager);

            var settings = config.Get<TestSettings>("Tenant");

            Assert.Multiple(() =>
            {
                Assert.That(settings.Key1, Is.EqualTo("Value1"));

                Assert.That(settings.Key2, Is.EqualTo(3));
            });
        }

        [Test]
        public void Get_WithTenantSettingInCategory_ReturnsTenantValue()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Category1:Key1", "Value1")
                .WithAppSetting("Category1:Key2", 2)
                .WithAppSetting("Tenant:Category1:Key2", 3)
                .Build();

            var config = new AppConfiguration(configManager);

            var settings = config.GetSection("Category1", "Tenant").Get<TestSettings>();

            Assert.Multiple(() =>
            {
                Assert.That(settings.Key1, Is.EqualTo("Value1"));

                Assert.That(settings.Key2, Is.EqualTo(3));
            });
        }

        [Test]
        public void Bind_WithTenantSetting_ReturnsTenantValue()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Key1", "Value1")
                .WithAppSetting("Key2", 2)
                .WithAppSetting("Tenant:Key2", 3)
                .Build();

            var config = new AppConfiguration(configManager);

            var settings = new TestSettings();
            config.Bind(tenant: "Tenant", settings);

            Assert.Multiple(() =>
            {
                Assert.That(settings.Key1, Is.EqualTo("Value1"));

                Assert.That(settings.Key2, Is.EqualTo(3));
            });
        }

        [Test]
        public void Bind_WithTenantSettingInCategory_ReturnsTenantValue()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Category1:Key1", "Value1")
                .WithAppSetting("Category1:Key2", 2)
                .WithAppSetting("Tenant:Category1:Key2", 3)
                .Build();

            var config = new AppConfiguration(configManager);

            var settings = new TestSettings();
            config.Bind("Category1", "Tenant", settings);

            Assert.Multiple(() =>
            {
                Assert.That(settings.Key1, Is.EqualTo("Value1"));

                Assert.That(settings.Key2, Is.EqualTo(3));
            });
        }
    }
}
