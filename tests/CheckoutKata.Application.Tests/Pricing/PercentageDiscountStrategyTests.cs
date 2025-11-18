using CheckoutKata.Application.Pricing.Strategies;
using FluentAssertions;

namespace CheckoutKata.Application.Tests.Pricing;

public class PercentageDiscountStrategyTests
{
    private readonly PercentageDiscountStrategy _strategy = new();

    [Fact]
    public void CalculatePrice_WhenBelowThreshold_ReturnsFullPrice()
    {
        var price = _strategy.CalculatePrice(2, 50, specialQuantity: 3, specialPrice: 10);

        price.Should().Be(100, because: "need 3+ items for discount");
    }

    [Fact]
    public void CalculatePrice_WhenAtThreshold_AppliesDiscount()
    {
        var price = _strategy.CalculatePrice(3, 50, specialQuantity: 3, specialPrice: 10);

        price.Should().Be(135, because: "3 × 50 = 150, 10% off = 135");
    }

    [Fact]
    public void CalculatePrice_WhenAboveThreshold_AppliesDiscount()
    {
        var price = _strategy.CalculatePrice(5, 50, specialQuantity: 3, specialPrice: 20);

        price.Should().Be(200, because: "5 × 50 = 250, 20% off = 200");
    }

    [Fact]
    public void CalculatePrice_WhenNoSpecialQuantity_ReturnsFullPrice()
    {
        var price = _strategy.CalculatePrice(5, 50);

        price.Should().Be(250);
    }
}