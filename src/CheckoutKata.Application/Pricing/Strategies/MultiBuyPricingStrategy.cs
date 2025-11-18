using CheckoutKata.Application.Pricing.Contracts;

namespace CheckoutKata.Application.Pricing.Strategies;

public class MultiBuyPricingStrategy : IPricingStrategy
{
    public int CalculatePrice(int quantity, int unitPrice, int? specialQuantity = null, int? specialPrice = null)
    {
        if (specialQuantity.HasValue && specialPrice.HasValue)
        {
            var specialSets = quantity / specialQuantity.Value;
            var remainder = quantity % specialQuantity.Value;
            return (specialSets * specialPrice.Value) + (remainder * unitPrice);
        }

        return quantity * unitPrice;
    }
}