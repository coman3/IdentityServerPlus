// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "ASP0000:Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'", Justification = "This is a requirement for services to be injected", Scope = "member", Target = "~M:IdentityServer.Startup.ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection)")]
