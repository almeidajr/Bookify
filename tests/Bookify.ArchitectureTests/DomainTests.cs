using Bookify.Domain.Abstractions;
using FluentAssertions;
using NetArchTest.Rules;

namespace Bookify.ArchitectureTests;

public class DomainTests : BaseTest
{
    [Fact]
    public void DomainEvent_ShouldHaveDomainEventPostfix()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}