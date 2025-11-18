namespace CheckoutKata.Application.Pricing.Models;

public record PricingRule(string Sku, int UnitPrice, int? SpecialQuantity = null, int? SpecialPrice = null);