namespace CheckoutKata.Application.Contracts;

public interface ICheckout
{
    void Scan(string item);
    int GetTotalPrice();
}
