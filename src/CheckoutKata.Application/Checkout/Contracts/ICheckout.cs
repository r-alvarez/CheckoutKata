namespace CheckoutKata.Application.Checkout.Contracts;

public interface ICheckout
{
    void Scan(string item);

    int GetTotalPrice();
}