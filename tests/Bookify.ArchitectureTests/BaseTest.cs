using System.Reflection;
using Bookify.Application;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure;

namespace Bookify.ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(IEntity).Assembly;

    protected static readonly Assembly ApplicationAssembly =
        typeof(ApplicationServiceCollectionExtensions).Assembly;

    protected static readonly Assembly InfrastructureAssembly =
        typeof(InfrastructureServiceCollectionExtensions).Assembly;
}