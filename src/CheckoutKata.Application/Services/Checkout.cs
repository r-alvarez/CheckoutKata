using CheckoutKata.Application.Contracts;
using CheckoutKata.Application.Models;

namespace CheckoutKata.Application.Services;

public class Checkout(IEnumerable<PricingRule> pricingRules) : ICheckout
{
    private readonly Dictionary<string, PricingRule> _rulesBySku = pricingRules.ToDictionary(r => r.Sku);
    private readonly Dictionary<string, int> _scannedItems = [];

    public void Scan(string item)
    {
        _scannedItems[item] = _scannedItems.GetValueOrDefault(item) + 1;
    }

    public int GetTotalPrice()
    {
        return _scannedItems.Sum(item =>
        {
            if (!_rulesBySku.TryGetValue(item.Key, out var rule))
                throw new ArgumentException($"Unknown SKU: {item.Key}", nameof(item));

            return rule.CalculatePrice(item.Value);
        });
    }
}
