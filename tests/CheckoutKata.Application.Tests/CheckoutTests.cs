using CheckoutKata.Application.Contracts;
using CheckoutKata.Application.Models;
using CheckoutKata.Application.Services;
using FluentAssertions;

namespace CheckoutKata.Application.Tests;

public class CheckoutTests
{
    [Fact]
    public void Scan_WhenCalled_DoesNotThrow()
    {
        var rules = Array.Empty<PricingRule>();
        ICheckout checkout = CreateCheckout(rules);

        var act = () => checkout.Scan("A");

        act.Should().NotThrow();
    }

    [Fact]
    public void GetTotalPrice_WhenNoItemsScanned_ReturnsZero()
    {
        var rules = Array.Empty<PricingRule>();
        ICheckout checkout = CreateCheckout(rules);

        var total = checkout.GetTotalPrice();

        total.Should().Be(0);
    }

    private static ICheckout CreateCheckout(IEnumerable<PricingRule> pricingRules) => new Checkout(pricingRules);
}
