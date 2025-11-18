using CheckoutKata.Application.Pricing.Models;

namespace CheckoutKata.Application.Pricing.Contracts;

public interface IPricingStrategyFactory
{
    IPricingStrategy GetStrategy(PricingStrategyType type);
}
