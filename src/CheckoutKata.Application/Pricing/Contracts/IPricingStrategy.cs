namespace CheckoutKata.Application.Pricing.Contracts;

public interface IPricingStrategy
{
    int CalculatePrice(int quantity, int unitPrice, int? specialQuantity = null, int? specialPrice = null);
}
