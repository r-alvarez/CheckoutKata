using CheckoutKata.Application.Pricing.Contracts;

namespace CheckoutKata.Application.Pricing.Strategies;

public class BuyOneGetOneFreeStrategy : IPricingStrategy
{
    public int CalculatePrice(int quantity, int unitPrice, int? specialQuantity = null, int? specialPrice = null)
    {
        // BOGOF: Customer pays for every other item
        // Examples: 2 items = pay for 1, 3 items = pay for 2, 4 items = pay for 2
        var paidItems = (quantity / 2) + (quantity % 2);
        return paidItems * unitPrice;
    }
}