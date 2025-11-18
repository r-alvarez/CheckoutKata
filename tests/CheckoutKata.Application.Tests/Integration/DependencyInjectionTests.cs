using CheckoutKata.Application.Checkout.Contracts;
using CheckoutKata.Application.Extensions;
using CheckoutKata.Application.Pricing.Contracts;
using CheckoutKata.Application.Pricing.Models;
using CheckoutKata.Application.Pricing.Strategies;
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

    [Fact]
    public void AddCheckoutServices_WithDifferentStrategy_UsesThatStrategy()
    {
        // Arrange
        var services = new ServiceCollection();
        var rules = new[] { new PricingRule("A", 50, specialQuantity: 2, specialPrice: 1) };
        services.AddCheckoutServices(rules, PricingStrategyType.BuyOneGetOneFree);
        var provider = services.BuildServiceProvider();
        var checkout = provider.GetRequiredService<ICheckout>();

        // Act
        checkout.Scan("A");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        // Assert
        total.Should().Be(50, because: "BOGOF strategy: buy 2, pay for 1");
    }

    [Fact]
    public void AddCheckoutServices_WithBuyOneGetOneFreeStrategy_CalculatesCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var rules = new[] { new PricingRule("A", 50, specialQuantity: 2, specialPrice: 1) };
        services.AddCheckoutServices(rules, PricingStrategyType.BuyOneGetOneFree);
        var provider = services.BuildServiceProvider();
        var checkout = provider.GetRequiredService<ICheckout>();

        // Act
        checkout.Scan("A");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        // Assert
        total.Should().Be(50, because: "BOGOF: 2 items, pay for 1");
    }

    [Fact]
    public void AddCheckoutServices_WithPercentageDiscount_AppliesCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var rules = new[] { new PricingRule("A", 50, 3, 20) };
        services.AddCheckoutServices(rules, PricingStrategyType.PercentageDiscount);
        var provider = services.BuildServiceProvider();
        var checkout = provider.GetRequiredService<ICheckout>();

        // Act
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        // Assert
        total.Should().Be(120, because: "3 × 50 = 150, 20% off = 120");
    }

    [Fact]
    public void AddCheckoutServices_WithMultiBuyStrategy_WorksAsDefault()
    {
        // Arrange
        var services = new ServiceCollection();
        var rules = new[] { new PricingRule("A", 50, 3, 130) };
        services.AddCheckoutServices(rules);
        var provider = services.BuildServiceProvider();
        var checkout = provider.GetRequiredService<ICheckout>();

        // Act
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        // Assert
        total.Should().Be(130, because: "default MultiBuy strategy: 3 for 130");
    }

    [Fact]
    public void AddCheckoutServices_RegistersFactory_CanResolveFactory()
    {
        // Arrange
        var services = new ServiceCollection();
        var rules = new[] { new PricingRule("A", 50) };
        services.AddCheckoutServices(rules);
        var provider = services.BuildServiceProvider();

        // Act
        var factory = provider.GetService<IPricingStrategyFactory>();

        // Assert
        factory.Should().NotBeNull();
        factory.Should().BeOfType<PricingStrategyFactory>();
    }
}