using CheckoutKata.Application.Pricing.Contracts;
using CheckoutKata.Application.Pricing.Models;

namespace CheckoutKata.Application.Pricing;

public class PricingService(IPricingStrategy pricingStrategy) : IPricingService
{
    public int CalculateTotal(Dictionary<string, int> scannedItems, IEnumerable<PricingRule> rules)
    {
        var rulesBySku = rules.ToDictionary(r => r.Sku);

        return scannedItems.Sum(item =>
        {
            if (!rulesBySku.TryGetValue(item.Key, out var rule))
                throw new ArgumentException($"Unknown SKU: {item.Key}");

            return pricingStrategy.CalculatePrice(
                item.Value,
                rule.UnitPrice,
                rule.SpecialQuantity,
                rule.SpecialPrice);
        });
    }
}