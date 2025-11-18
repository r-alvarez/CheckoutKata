using CheckoutKata.Application.Pricing.Contracts;

namespace CheckoutKata.Application.Pricing.Strategies;

public class BuyOneGetOneFreeStrategy : IPricingStrategy
{
    public int CalculatePrice(int quantity, int unitPrice, int? specialQuantity = null, int? specialPrice = null)
    {
        if (specialQuantity is null)
            return quantity * unitPrice;

        var paidItems = (quantity / 2) + (quantity % 2);
        return paidItems * unitPrice;
    }
}