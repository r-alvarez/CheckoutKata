using CheckoutKata.Application.Pricing.Strategies;
using FluentAssertions;

namespace CheckoutKata.Application.Tests.Pricing;

public class BuyOneGetOneFreeStrategyTests
{
    private readonly BuyOneGetOneFreeStrategy _strategy = new();

    [Fact]
    public void CalculatePrice_WhenOneItem_ReturnsUnitPrice()
    {
        var price = _strategy.CalculatePrice(1, 50);

        price.Should().Be(50);
    }

    [Fact]
    public void CalculatePrice_WhenTwoItems_ReturnsOneUnitPrice()
    {
        var price = _strategy.CalculatePrice(2, 50, 2, 0);

        price.Should().Be(50, because: "buy 2, pay for 1");
    }

    [Fact]
    public void CalculatePrice_WhenThreeItems_ReturnsTwoUnitPrices()
    {
        var price = _strategy.CalculatePrice(3, 50, 2, 0);

        price.Should().Be(100, because: "buy 3, pay for 2");
    }

    [Fact]
    public void CalculatePrice_WhenFourItems_ReturnsTwoUnitPrices()
    {
        var price = _strategy.CalculatePrice(4, 50, 2, 0);

        price.Should().Be(100, because: "buy 4, pay for 2");
    }

    [Fact]
    public void CalculatePrice_WhenFiveItems_ReturnsThreeUnitPrices()
    {
        var price = _strategy.CalculatePrice(5, 50, 2, 0);

        price.Should().Be(150, because: "buy 5, pay for 3");
    }
}