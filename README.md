# Meeg.Configuration.Extensions

[![Build Status](https://dev.azure.com/cpmeeg/Meeg/_apis/build/status/meeg-configuration-extensions?branchName=develop)](https://dev.azure.com/cpmeeg/Meeg/_build/latest?definitionId=6&branchName=develop)

This library extends the configuration API of [Meeg.Configuration](https://github.com/CMeeg/meeg-configuration) by adding functionality to be used when working with [multi-tenant applications](#multi-tenant-applications).

## Getting started

To get started:

* Install the NuGet package `Meeg.Configuration.Extensions`
  * This will add the `Meeg.Configuration` package as a dependency
* Start [using the library](#usage) in your application code

## Usage

> Please read the [Meeg.Configuration](https://github.com/CMeeg/meeg-configuration) documentation first, which is the foundation for using this library. Then read on for usage instructions and guidance about the functions provided by this library.

### Multi-tenant applications

The general concept around multi-tenancy support provided by this library is that you:

* Define "default" configuration values using the standard configuration key format
* Define "tenant" configuration values that override the "default" values by prefixing the "default" key with the tenant name (or key, or id, or whatever uniquely identifies the tenant and can be used in the config key)
  * The tenant name is basically the first "section" in the key - see the example below
* Fetch configuration values using the extension methods to `IAppConfigurationRoot` provided by this library by using the overloads that allow you to specify the tenant name
* If a "tenant" value exists for the requested key and tenant it will be returned; else the "default" value will be returned

For example:

```c#
using Meeg.Configuration;
using Meeg.Configuration.Extensions.MultiTenant;

// Given these appSettings
// <appSettings>
//   <add key="Key" value="DefaultValue" />
//   <add key="Tenant:Key" value="TenantValue" />
//   <add key="OtherKey" value="DefaultValue" />
// </appSettings>

// We need an IAppConfigurationRoot instance to work with - see the Meeg.Configuration docs
var configManager = new ConfigurationManagerAdapter();
var config = new AppConfiguration(configManager);

// This will return "TenantValue"
config.GetValue("Key", tenant: "Tenant");

// This will return "DefaultValue"
config.GetValue("OtherKey", tenant: "Tenant");
```

Extension methods are provided for all of the main API features of `Meeg.Configuration`:

* `GetValue(string key, string tenant)`
* `GetValue(string key, string tenant, string defaultValue)`
* `GetValue<T>(string key, string tenant)`
* `GetValue<T>(string key, string tenant, T defaultValue)`
* `GetSection(string key, string tenant)`
* `GetChildren(string tenant)`
* `Get<T>(string tenant)`
* `Bind(string key, string tenant, object instance)`
