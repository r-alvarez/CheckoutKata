namespace CheckoutKata.Application.Models;

public record PricingRule(string Sku, int UnitPrice, int? SpecialQuantity = null, int? SpecialPrice = null)
{
    public int CalculatePrice(int quantity)
    {
        if (SpecialQuantity.HasValue && SpecialPrice.HasValue)
        {
            var specialSets = quantity / SpecialQuantity.Value;
            var remainder = quantity % SpecialQuantity.Value;
            return (specialSets * SpecialPrice.Value) + (remainder * UnitPrice);
        }

        return quantity * UnitPrice;
    }
}
