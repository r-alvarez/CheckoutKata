using CheckoutKata.Application.Pricing.Strategies;
using FluentAssertions;

namespace CheckoutKata.Application.Tests.Pricing;

public class PricingStrategyFactoryTests
{
    [Fact]
    public void GetStrategy_WhenMultiBuyType_ReturnsMultiBuyStrategy()
    {
        var factory = new PricingStrategyFactory();

        var strategy = factory.GetStrategy(PricingStrategyType.MultiBuy);

        strategy.Should().BeOfType<MultiBuyPricingStrategy>();
    }

    [Fact]
    public void GetStrategy_WhenBuyOneGetOneFreeType_ReturnsBuyOneGetOneFreeStrategy()
    {
        var factory = new PricingStrategyFactory();

        var strategy = factory.GetStrategy(PricingStrategyType.BuyOneGetOneFree);

        strategy.Should().BeOfType<BuyOneGetOneFreeStrategy>();
    }

    [Fact]
    public void GetStrategy_WhenPercentageDiscountType_ReturnsPercentageDiscountStrategy()
    {
        var factory = new PricingStrategyFactory();

        var strategy = factory.GetStrategy(PricingStrategyType.PercentageDiscount);

        strategy.Should().BeOfType<PercentageDiscountStrategy>();
    }

    [Fact]
    public void GetStrategy_WhenUnknownType_ThrowsArgumentException()
    {
        var factory = new PricingStrategyFactory();

        var act = () => factory.GetStrategy((PricingStrategyType)999);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Unknown strategy type*");
    }

    [Fact]
    public void GetStrategy_CalledMultipleTimes_ReturnsSameInstance()
    {
        var factory = new PricingStrategyFactory();

        var strategy1 = factory.GetStrategy(PricingStrategyType.MultiBuy);
        var strategy2 = factory.GetStrategy(PricingStrategyType.MultiBuy);

        strategy1.Should().BeSameAs(strategy2, "strategies should be cached/singleton");
    }
}