using CheckoutKata.Application.Pricing.Models;

namespace CheckoutKata.Application.Pricing.Contracts;

public interface IPricingService
{
    int CalculateTotal(Dictionary<string, int> scannedItems, IEnumerable<PricingRule> rules);
}