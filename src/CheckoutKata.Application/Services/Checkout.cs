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

    public int GetTotalPrice() => _scannedItems.Sum(item => _rulesBySku[item.Key].CalculatePrice(item.Value));
}
