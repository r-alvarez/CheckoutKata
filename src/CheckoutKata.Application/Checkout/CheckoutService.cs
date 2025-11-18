using CheckoutKata.Application.Checkout.Contracts;
using CheckoutKata.Application.Exceptions;
using CheckoutKata.Application.Pricing.Contracts;
using CheckoutKata.Application.Pricing.Models;

namespace CheckoutKata.Application.Checkout;

public class CheckoutService(IEnumerable<PricingRule> pricingRules, IPricingService pricingService) : ICheckout
{
    private readonly IEnumerable<PricingRule> _pricingRules = pricingRules;
    private readonly Dictionary<string, int> _scannedItems = [];

    public void Scan(string item)
    {
        if (string.IsNullOrWhiteSpace(item))
            throw new InvalidSkuException("SKU cannot be null or empty");

        if (!_scannedItems.TryAdd(item, 1))
            _scannedItems[item]++;
    }

    public int GetTotalPrice() => pricingService.CalculateTotal(_scannedItems, _pricingRules);
}