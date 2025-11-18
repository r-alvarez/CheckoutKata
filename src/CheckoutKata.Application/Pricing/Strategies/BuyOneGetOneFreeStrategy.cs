using CheckoutKata.Application.Pricing.Contracts;

namespace CheckoutKata.Application.Pricing.Strategies;

public class BuyOneGetOneFreeStrategy : IPricingStrategy
{
    public int CalculatePrice(int quantity, int unitPrice, int? specialQuantity = null, int? specialPrice = null)
    {
        // BOGOF is always active - buy 2, pay for 1
        var paidItems = (quantity / 2) + (quantity % 2);
        return paidItems * unitPrice;
    }
}