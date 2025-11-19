using CheckoutKata.Application.Pricing.Contracts;

namespace CheckoutKata.Application.Pricing.Strategies;

public class MultiBuyPricingStrategy : IPricingStrategy
{
    public int CalculatePrice(int quantity, int unitPrice, int? specialQuantity = null, int? specialPrice = null)
    {
        if (specialQuantity is not null && specialPrice is not null)
        {
            // Calculate multi-buy discount using division to avoid loops
            // Example: 5 items with "3 for 130" offer:
            //   - specialSets = 5 / 3 = 1 (one complete set of 3)
            //   - remainder = 5 % 3 = 2 (two items at unit price)
            //   - total = (1 × 130) + (2 × 50) = 230

            var specialSets = quantity / specialQuantity.Value;
            var remainder = quantity % specialQuantity.Value;
            return (specialSets * specialPrice.Value) + (remainder * unitPrice);
        }

        return quantity * unitPrice;
    }
}