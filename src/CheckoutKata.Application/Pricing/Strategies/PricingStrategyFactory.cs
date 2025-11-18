using CheckoutKata.Application.Pricing.Contracts;
using CheckoutKata.Application.Pricing.Models;

namespace CheckoutKata.Application.Pricing.Strategies;

public class PricingStrategyFactory : IPricingStrategyFactory
{
    private readonly Dictionary<PricingStrategyType, IPricingStrategy> _strategies = new()
    {
        { PricingStrategyType.MultiBuy, new MultiBuyPricingStrategy() },
        { PricingStrategyType.BuyOneGetOneFree, new BuyOneGetOneFreeStrategy() },
        { PricingStrategyType.PercentageDiscount, new PercentageDiscountStrategy() }
    };

    public IPricingStrategy GetStrategy(PricingStrategyType type)
    {
        if (_strategies.TryGetValue(type, out var strategy))
            return strategy;

        throw new ArgumentException($"Unknown strategy type: {type}", nameof(type));
    }
}