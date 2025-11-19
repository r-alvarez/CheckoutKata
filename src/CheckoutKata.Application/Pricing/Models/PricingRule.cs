using CheckoutKata.Application.Exceptions;

namespace CheckoutKata.Application.Pricing.Models;

public record PricingRule
{
    public string Sku { get; }
    public int UnitPrice { get; }
    public int? SpecialQuantity { get; }
    public int? SpecialPrice { get; }

    public PricingRule(string sku, int unitPrice, int? specialQuantity = null, int? specialPrice = null)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new InvalidPricingRuleException("SKU cannot be null or empty");

        if (unitPrice is <= 0)
            throw new InvalidPricingRuleException("Unit price must be greater than zero");

        if ((specialQuantity is not null && specialPrice is null) ||
            (specialQuantity is null && specialPrice is not null))
            throw new InvalidPricingRuleException("Special quantity and special price must both be provided together");

        if (specialQuantity is <= 0)
            throw new InvalidPricingRuleException("Special quantity must be greater than zero");

        if (specialPrice is <= 0)
            throw new InvalidPricingRuleException("Special price must be greater than zero");

        if (specialQuantity is not null && specialPrice is not null)
        {
            // Business rule: Special offers must provide actual savings
            // A "special" price that costs the same or more makes no sense
            var regularPrice = unitPrice * specialQuantity.Value;
            if (specialPrice.Value >= regularPrice)
                throw new InvalidPricingRuleException(
                    $"Special price ({specialPrice.Value}) must be less than the regular price ({regularPrice}) for {specialQuantity.Value} items");
        }

        Sku = sku;
        UnitPrice = unitPrice;
        SpecialQuantity = specialQuantity;
        SpecialPrice = specialPrice;
    }
}