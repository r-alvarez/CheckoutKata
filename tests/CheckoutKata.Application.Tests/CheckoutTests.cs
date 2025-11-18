using CheckoutKata.Application.Contracts;
using CheckoutKata.Application.Models;
using CheckoutKata.Application.Services;
using FluentAssertions;

namespace CheckoutKata.Application.Tests;

public class CheckoutTests
{
    private readonly Func<IEnumerable<PricingRule>, ICheckout> _createCheckout;

    public CheckoutTests()
    {
        _createCheckout = rules => new Checkout(rules);
    }

    [Fact]
    public void Scan_WhenCalled_DoesNotThrow()
    {
        var rules = Array.Empty<PricingRule>();
        ICheckout checkout = _createCheckout(rules);

        var act = () => checkout.Scan("A");

        act.Should().NotThrow();
    }

    [Fact]
    public void GetTotalPrice_WhenNoItemsScanned_ReturnsZero()
    {
        var rules = Array.Empty<PricingRule>();
        ICheckout checkout = _createCheckout(rules);

        var total = checkout.GetTotalPrice();

        total.Should().Be(0);
    }

    [Fact]
    public void GetTotalPrice_WhenSingleItemScanned_ReturnsUnitPrice()
    {
        var rules = new[] { new PricingRule("C", UnitPrice: 20) };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("C");
        var total = checkout.GetTotalPrice();

        total.Should().Be(20);
    }

    [Fact]
    public void GetTotalPrice_WhenThreeItemsWithSpecialOffer_ReturnsSpecialPrice()
    {
        var rules = new[] { new PricingRule("A", UnitPrice: 50, SpecialQuantity: 3, SpecialPrice: 130) };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        total.Should().Be(130, because: "3 items of A should cost 130, not 150");
    }

    [Fact]
    public void GetTotalPrice_WhenItemsScannedInAnyOrder_CalculatesCorrectly()
    {
        var rules = new[]
        {
            new PricingRule("A", UnitPrice: 50, SpecialQuantity: 3, SpecialPrice: 130),
            new PricingRule("B", UnitPrice: 30, SpecialQuantity: 2, SpecialPrice: 45),
            new PricingRule("C", UnitPrice: 20),
            new PricingRule("D", UnitPrice: 15)
        };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("B");
        checkout.Scan("A");
        checkout.Scan("B");
        var total = checkout.GetTotalPrice();

        total.Should().Be(95, because: "B+A+B should be: 2xB(45) + 1xA(50) = 95");
    }

    [Fact]
    public void GetTotalPrice_WhenUnknownItemScanned_ThrowsArgumentException()
    {
        var rules = new[]
        {
            new PricingRule("A", UnitPrice: 50)
        };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("Z");
        var act = () => checkout.GetTotalPrice();

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Unknown SKU*Z*");
    }

}
