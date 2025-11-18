namespace CheckoutKata.Application.Exceptions;

public class InvalidPricingRuleException(string message) : ArgumentException(message);
