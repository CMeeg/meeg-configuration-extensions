using System.Linq;
using NUnit.Framework;
using Meeg.Configuration.Extensions.MultiTenant;

namespace Meeg.Configuration.Extensions.Tests.MultiTenant
{
    [TestFixture]
    public class AppConfigurationExtensionsTests
    {
        [Test]
        public void GetValue_WithGlobalOnlySetting_ReturnsGlobalValue()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Key", "Value1")
                .WithAppSetting("Category:Key", "Value2")
                .Build();

            var config = new AppConfiguration(configManager);

            Assert.Multiple(() =>
            {
                Assert.That(config.GetValue("Key", tenant: "Tenant"), Is.EqualTo("Value1"));

                Assert.That(config.GetValue("Category:Key", tenant: "Tenant"), Is.EqualTo("Value2"));
            });
        }

        [Test]
        public void GetValue_WithTenantSetting_ReturnsTenantValue()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Key", "Value1")
                .WithAppSetting("Category:Key", "Value2")
                .WithAppSetting("Tenant:Key", "Value3")
                .WithAppSetting("Tenant:Category:Key", "Value4")
                .Build();

            var config = new AppConfiguration(configManager);

            Assert.Multiple(() =>
            {
                Assert.That(config.GetValue("Key", tenant: "Tenant"), Is.EqualTo("Value3"));

                Assert.That(config.GetValue("Category:Key", tenant: "Tenant"), Is.EqualTo("Value4"));
            });
        }

        [Test]
        public void GetValue_WithDefaultValue_ReturnsDefaultValue()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Key", "Value1")
                .WithAppSetting("Category:Key", "Value2")
                .WithAppSetting("Tenant:Key", null)
                .WithAppSetting("Tenant:Category:Key", null)
                .WithAppSetting("GlobalKey", null)
                .WithAppSetting("Category:GlobalKey", null)
                .Build();

            var config = new AppConfiguration(configManager);

            Assert.Multiple(() =>
            {
                Assert.That(config.GetValue("Key", "Tenant", "DefaultValue"), Is.EqualTo("DefaultValue"));

                Assert.That(config.GetValue("Category:Key", "Tenant", "DefaultValue"), Is.EqualTo("DefaultValue"));

                Assert.That(config.GetValue("GlobalKey", "Tenant", "DefaultValue"), Is.EqualTo("DefaultValue"));

                Assert.That(config.GetValue("Category:GlobalKey", "Tenant", "DefaultValue"), Is.EqualTo("DefaultValue"));
            });
        }

        [Test]
        public void GetSection_WithGlobalOnlySection_ReturnsGlobalSection()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Key", "Value1")
                .WithAppSetting("Category:Key1", "Value2")
                .WithAppSetting("Category:Key2", "Value3")
                .Build();

            var config = new AppConfiguration(configManager);

            Assert.Multiple(() =>
            {
                Assert.That(config.GetSection("Key", "Tenant").Value, Is.EqualTo("Value1"));

                Assert.That(config.GetSection("Category", "Tenant")["Key1"], Is.EqualTo("Value2"));

                Assert.That(config.GetSection("Category", "Tenant")["Key2"], Is.EqualTo("Value3"));
            });
        }

        [Test]
        public void GetSection_WithTenantSection_ReturnsTenantSection()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Key", "Value1")
                .WithAppSetting("Category:Key1", "Value2")
                .WithAppSetting("Category:Key2", "Value3")
                .WithAppSetting("Tenant:Key", "Value4")
                .WithAppSetting("Tenant:Category:Key1", "Value5")
                .WithAppSetting("Tenant:Category:Key2", null)
                .Build();

            var config = new AppConfiguration(configManager);

            Assert.Multiple(() =>
            {
                Assert.That(config.GetSection("Key", "Tenant").Value, Is.EqualTo("Value4"));

                Assert.That(config.GetSection("Category", "Tenant")["Key1"], Is.EqualTo("Value5"));

                Assert.That(config.GetSection("Category", "Tenant")["Key2"], Is.EqualTo(null));
            });
        }

        [Test]
        public void GetSection_WithTenantSection_ReturnsGlobalValueForMissingTenantKey()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Key", "Value1")
                .WithAppSetting("Category1:Key1", "Value2")
                .WithAppSetting("Category1:Key2", "Value3")
                .WithAppSetting("Category1:Category2:Key1", "Value4")
                .WithAppSetting("Category1:Category2:Key2", "Value5")
                .WithAppSetting("Tenant:Category1:Key1", "Value6")
                .WithAppSetting("Tenant:Category1:Category2:Key1", "Value7")
                .Build();

            var config = new AppConfiguration(configManager);

            Assert.Multiple(() =>
            {
                Assert.That(config.GetSection("Key", "Tenant").Value, Is.EqualTo("Value1"));

                var categorySection1 = config.GetSection("Category1", "Tenant");

                Assert.That(categorySection1["Key2"], Is.EqualTo("Value3"));

                Assert.That(categorySection1.GetSection("Category2")["Key2"], Is.EqualTo("Value5"));
            });
        }

        [Test]
        public void GetChildren_WithSection_ReturnsGlobalChildrenForMissingTenantChildren()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Category1:Key1", "Value1")
                .WithAppSetting("Category1:Key2", "Value2")
                .WithAppSetting("Category1:Key3", "Value3")
                .WithAppSetting("Category1:Category2:Key1", "Value4")
                .WithAppSetting("Category1:Category2:Key2", "Value5")
                .WithAppSetting("Category3:Key1", "Value6")
                .WithAppSetting("Tenant:Category1:Key1", "Value7")
                .WithAppSetting("Tenant:Category1:Key3", "Value8")
                .WithAppSetting("Tenant:Category1:Category2:Key2", "Value9")
                .Build();

            var config = new AppConfiguration(configManager);

            Assert.Multiple(() =>
            {
                var children = config.GetSection("Category1", "Tenant")
                    .GetChildren()
                    .ToList();

                // All global children of the specified section should be represented

                Assert.That(children.Count, Is.EqualTo(5));

                // If a tenant specific key is not present it should fall back to the global equivalent

                Assert.That(children.Any(child => child.Key == "Key2"), Is.True);

                Assert.That(children.FirstOrDefault(child => child.Key == "Key2")?.Value, Is.EqualTo("Value2"));
            });
        }

        [Test]
        public void GetChildren_WithTenantAsSection_ReturnsTenantChildrenWhenAvailable()
        {
            var configManager = new ConfigurationManagerFixture()
                .WithAppSetting("Key1", "Value1")
                .WithAppSetting("Key2", "Value2")
                .WithAppSetting("Category1:Key1", "Value3")
                .WithAppSetting("Category1:Key2", "Value4")
                .WithAppSetting("Category1:Key3", "Value5")
                .WithAppSetting("Category1:Category2:Key1", "Value6")
                .WithAppSetting("Category1:Category2:Key2", "Value7")
                .WithAppSetting("Category3:Key1", "Value8")
                .WithAppSetting("Tenant:Key2", "Value9")
                .WithAppSetting("Tenant:Key3", "Value10") // This has no global equivalent so should not be included as a child
                .WithAppSetting("Tenant:Category1:Key1", "Value11")
                .WithAppSetting("Tenant:Category1:Key3", "Value12")
                .WithAppSetting("Tenant:Category1:Category2:Key2", "Value13")
                .WithAppSetting("Tenants:Category1:Key1", "Value14") // This starts with the Tenant, but does not equal it and should be included as a child
                .Build();

            var config = new AppConfiguration(configManager);

            Assert.Multiple(() =>
            {
                var children = config.GetChildren("Tenant")
                    .ToList();

                Assert.That(children.Count, Is.EqualTo(9));

                Assert.That(children.FirstOrDefault(child => child.Path == "Tenant:Key2")?.Value, Is.EqualTo("Value9"));
            });
        }
    }
}
