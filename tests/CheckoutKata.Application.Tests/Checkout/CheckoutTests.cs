using CheckoutKata.Application.Checkout;
using CheckoutKata.Application.Checkout.Contracts;
using CheckoutKata.Application.Exceptions;
using CheckoutKata.Application.Pricing;
using CheckoutKata.Application.Pricing.Contracts;
using CheckoutKata.Application.Pricing.Models;
using CheckoutKata.Application.Pricing.Strategies;
using FluentAssertions;

namespace CheckoutKata.Application.Tests.Checkout;

public class CheckoutTests
{
    private readonly Func<IEnumerable<PricingRule>, ICheckout> _createCheckout;
    private readonly IPricingService _pricingService;

    public CheckoutTests()
    {
        var pricingStrategy = new MultiBuyPricingStrategy();
        _pricingService = new PricingService(pricingStrategy);
        _createCheckout = rules => new CheckoutService(rules, _pricingService);
    }

    [Fact]
    public void Scan_WhenCalled_DoesNotThrow()
    {
        var rules = Array.Empty<PricingRule>();
        ICheckout checkout = _createCheckout(rules);

        var act = () => checkout.Scan("A");

        act.Should().NotThrow();
    }

    [Fact]
    public void GetTotalPrice_WhenNoItemsScanned_ReturnsZero()
    {
        var rules = Array.Empty<PricingRule>();
        ICheckout checkout = _createCheckout(rules);

        var total = checkout.GetTotalPrice();

        total.Should().Be(0);
    }

    [Fact]
    public void GetTotalPrice_WhenSingleItemScanned_ReturnsUnitPrice()
    {
        var rules = new PricingRule[] { new("C", unitPrice: 20) };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("C");
        var total = checkout.GetTotalPrice();

        total.Should().Be(20);
    }

    [Fact]
    public void GetTotalPrice_WhenSingleItemA_ReturnsUnitPrice()
    {
        var rules = new PricingRule[] { new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130) };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        total.Should().Be(50);
    }

    [Fact]
    public void GetTotalPrice_WhenTwoItemsA_ReturnsUnitPrices()
    {
        var rules = new PricingRule[] { new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130) };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("A");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        total.Should().Be(100, because: "2 A's at 50 each = 100");
    }

    [Fact]
    public void GetTotalPrice_WhenThreeItemsWithSpecialOffer_ReturnsSpecialPrice()
    {
        var rules = new PricingRule[] { new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130) };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        total.Should().Be(130, because: "3 A's trigger special offer = 130");
    }

    [Fact]
    public void GetTotalPrice_WhenFourItemsA_ReturnsOneSpecialPlusOne()
    {
        var rules = new PricingRule[] { new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130) };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        total.Should().Be(180, because: "3 A's (130) + 1 A (50) = 180");
    }

    [Fact]
    public void GetTotalPrice_WhenFiveItemsA_ReturnsOneSpecialPlusTwo()
    {
        var rules = new PricingRule[] { new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130) };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        total.Should().Be(230, because: "3 A's (130) + 2 A's (100) = 230");
    }

    [Fact]
    public void GetTotalPrice_WhenSixItemsA_ReturnsTwoSpecials()
    {
        var rules = new PricingRule[] { new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130) };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        total.Should().Be(260, because: "3 A's (130) + 3 A's (130) = 260");
    }

    [Fact]
    public void GetTotalPrice_WhenMixedItems_CalculatesCorrectly()
    {
        var rules = new PricingRule[]
        {
            new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130),
            new("B", unitPrice: 30, specialQuantity: 2, specialPrice: 45),
            new("C", unitPrice: 20),
            new("D", unitPrice: 15)
        };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("A");
        checkout.Scan("B");
        var total = checkout.GetTotalPrice();

        total.Should().Be(80, because: "A (50) + B (30) = 80");
    }

    [Fact]
    public void GetTotalPrice_WhenItemsScannedInAnyOrder_CalculatesCorrectly()
    {
        var rules = new PricingRule[]
        {
            new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130),
            new("B", unitPrice: 30, specialQuantity: 2, specialPrice: 45),
            new("C", unitPrice: 20),
            new("D", unitPrice: 15)
        };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("B");
        checkout.Scan("A");
        checkout.Scan("B");
        var total = checkout.GetTotalPrice();

        total.Should().Be(95, because: "2 B's (45) + 1 A (50) = 95");
    }

    [Fact]
    public void GetTotalPrice_WhenComplexMixAABB_CalculatesCorrectly()
    {
        var rules = new PricingRule[]
        {
            new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130),
            new("B", unitPrice: 30, specialQuantity: 2, specialPrice: 45),
            new("C", unitPrice: 20),
            new("D", unitPrice: 15)
        };

        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("B");
        checkout.Scan("B");
        var total = checkout.GetTotalPrice();

        total.Should().Be(175, because: "3 A's (130) + 2 B's (45) = 175");
    }

    [Fact]
    public void GetTotalPrice_WhenComplexMixAAABBD_CalculatesCorrectly()
    {
        var rules = new PricingRule[]
        {
            new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130),
            new("B", unitPrice: 30, specialQuantity: 2, specialPrice: 45),
            new("C", unitPrice: 20),
            new("D", unitPrice: 15)
        };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("B");
        checkout.Scan("B");
        checkout.Scan("D");
        var total = checkout.GetTotalPrice();

        total.Should().Be(190, because: "3 A's (130) + 2 B's (45) + D (15) = 190");
    }

    [Fact]
    public void GetTotalPrice_WhenScannedInRandomOrder_CalculatesCorrectly()
    {
        var rules = new PricingRule[]
        {
            new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130),
            new("B", unitPrice: 30, specialQuantity: 2, specialPrice: 45),
            new("C", unitPrice: 20),
            new("D", unitPrice: 15)
        };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("D");
        checkout.Scan("A");
        checkout.Scan("B");
        checkout.Scan("A");
        checkout.Scan("B");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        total.Should().Be(190, because: "D (15) + 3 A's (130) + 2 B's (45) = 190");
    }

    [Fact]
    public void GetTotalPrice_WhenMixedCDBA_CalculatesCorrectly()
    {
        var rules = new PricingRule[]
        {
            new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130),
            new("B", unitPrice: 30, specialQuantity: 2, specialPrice: 45),
            new("C", unitPrice: 20),
            new("D", unitPrice: 15)
        };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("C");
        checkout.Scan("D");
        checkout.Scan("B");
        checkout.Scan("A");
        var total = checkout.GetTotalPrice();

        total.Should().Be(115, because: "C (20) + D (15) + B (30) + A (50) = 115");
    }

    [Fact]
    public void GetTotalPrice_WhenCalledMultipleTimesDuringScan_ReturnsCorrectRunningTotal()
    {
        var rules = new PricingRule[]
        {
            new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130),
            new("B", unitPrice: 30, specialQuantity: 2, specialPrice: 45)
        };

        ICheckout checkout = _createCheckout(rules);

        checkout.GetTotalPrice().Should().Be(0);

        checkout.Scan("A");
        checkout.GetTotalPrice().Should().Be(50);

        checkout.Scan("B");
        checkout.GetTotalPrice().Should().Be(80);

        checkout.Scan("A");
        checkout.GetTotalPrice().Should().Be(130, because: "2 A's (100) + 1 B (30) = 130");

        checkout.Scan("A");
        checkout.GetTotalPrice().Should().Be(160, because: "3 A's trigger special (130) + 1 B (30) = 160");

        checkout.Scan("B");
        checkout.GetTotalPrice().Should().Be(175, because: "3 A's (130) + 2 B's trigger special (45) = 175");
    }

    [Fact]
    public void GetTotalPrice_WhenThreeItemsB_ReturnsSpecialPlusOne()
    {
        var rules = new PricingRule[] { new("B", unitPrice: 30, specialQuantity: 2, specialPrice: 45) };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("B");
        checkout.Scan("B");
        checkout.Scan("B");
        var total = checkout.GetTotalPrice();

        total.Should().Be(75, because: "2 B's (45) + 1 B (30) = 75");
    }

    [Fact]
    public void GetTotalPrice_WhenUnknownItemScanned_ThrowsUnknownSkuException()
    {
        var rules = new PricingRule[] { new("A", unitPrice: 50) };
        ICheckout checkout = _createCheckout(rules);

        checkout.Scan("Z");
        var act = checkout.GetTotalPrice;

        act.Should().Throw<UnknownSkuException>()
            .WithMessage("*Unknown SKU 'Z'*")
            .And.Sku.Should().Be("Z");
    }

    [Fact]
    public void Scan_WhenSkuIsNull_ThrowsInvalidSkuException()
    {
        var rules = Array.Empty<PricingRule>();
        ICheckout checkout = _createCheckout(rules);
        var emptySku = string.Empty;

        var act = () => checkout.Scan(emptySku);

        act.Should().Throw<InvalidSkuException>()
            .WithMessage("*SKU cannot be null or empty*");
    }

    [Fact]
    public void Scan_WhenSkuIsEmpty_ThrowsInvalidSkuException()
    {
        var rules = Array.Empty<PricingRule>();
        ICheckout checkout = _createCheckout(rules);

        var nullSku = (string?)null;

        var act = () => checkout.Scan(nullSku!);

        act.Should().Throw<InvalidSkuException>()
            .WithMessage("*SKU cannot be null or empty*");
    }

    [Fact]
    public void Scan_WhenSkuIsWhitespace_ThrowsInvalidSkuException()
    {
        var rules = Array.Empty<PricingRule>();
        ICheckout checkout = _createCheckout(rules);

        var act = () => checkout.Scan("   ");

        act.Should().Throw<InvalidSkuException>()
            .WithMessage("*SKU cannot be null or empty*");
    }
}