using CheckoutKata.Application.Checkout.Contracts;
using CheckoutKata.Application.Extensions;
using CheckoutKata.Application.Pricing.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CheckoutKata.Application.Tests.Integration;

public class DependencyInjectionTests
{
    [Fact]
    public void AddCheckoutServices_RegistersAllDependencies_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var rules = new[]
        {
            new PricingRule("A", 50, 3, 130),
            new PricingRule("B", 30, 2, 45)
        };

        // Act
        services.AddCheckoutServices(rules);
        var provider = services.BuildServiceProvider();

        // Assert
        var checkout = provider.GetService<ICheckout>();
        checkout.Should().NotBeNull();
    }

    [Fact]
    public void AddCheckoutServices_CheckoutIsScoped_NewInstancePerScope()
    {
        // Arrange
        var services = new ServiceCollection();
        var rules = new[] { new PricingRule("A", 50) };
        services.AddCheckoutServices(rules);
        var provider = services.BuildServiceProvider();

        // Act
        ICheckout checkout1;
        ICheckout checkout2;
        ICheckout checkout3;

        using (var scope1 = provider.CreateScope())
        {
            checkout1 = scope1.ServiceProvider.GetRequiredService<ICheckout>();
            checkout2 = scope1.ServiceProvider.GetRequiredService<ICheckout>();
        }

        using (var scope2 = provider.CreateScope())
        {
            checkout3 = scope2.ServiceProvider.GetRequiredService<ICheckout>();
        }

        // Assert
        checkout1.Should().BeSameAs(checkout2, "same scope should return same instance");
        checkout1.Should().NotBeSameAs(checkout3, "different scope should return different instance");
    }

    [Fact]
    public void CheckoutService_WithDependencyInjection_CalculatesCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var rules = new[]
        {
            new PricingRule("A", 50, 3, 130),
            new PricingRule("B", 30, 2, 45)
        };

        services.AddCheckoutServices(rules);
        var provider = services.BuildServiceProvider();
        var checkout = provider.GetRequiredService<ICheckout>();

        // Act
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("B");
        checkout.Scan("B");
        var total = checkout.GetTotalPrice();

        // Assert
        total.Should().Be(175);
    }
}
