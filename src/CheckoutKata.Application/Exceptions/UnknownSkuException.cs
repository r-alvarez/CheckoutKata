namespace CheckoutKata.Application.Exceptions;

public class UnknownSkuException(string sku) : Exception($"Unknown SKU '{sku}'. This item does not exist in the pricing rules.")
{
    public string Sku { get; } = sku;
}