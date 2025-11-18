using CheckoutKata.Application.Pricing.Contracts;

namespace CheckoutKata.Application.Pricing.Strategies;

public class PercentageDiscountStrategy : IPricingStrategy
{
    public int CalculatePrice(int quantity, int unitPrice, int? specialQuantity = null, int? specialPrice = null)
    {
        var total = quantity * unitPrice;

        if (specialQuantity is not null && quantity >= specialQuantity.Value && specialPrice is not null)
        {
            var discountPercent = specialPrice.Value;
            return total - (total * discountPercent / 100);
        }

        return total;
    }
}