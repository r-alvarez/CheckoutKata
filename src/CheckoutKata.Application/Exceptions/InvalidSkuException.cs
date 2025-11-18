namespace CheckoutKata.Application.Exceptions;

public class InvalidSkuException(string message) : ArgumentException(message);