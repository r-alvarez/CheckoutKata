using CheckoutKata.Application.Exceptions;
using CheckoutKata.Application.Pricing.Models;
using FluentAssertions;

namespace CheckoutKata.Application.Tests.Pricing;

public class PricingRuleTests
{
    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        var act = () => new PricingRule("A", 50);

        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithValidSpecialOffer_CreatesInstance()
    {
        var act = () => new PricingRule("A", 50, 3, 130);

        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WhenSkuIsNull_ThrowsInvalidPricingRuleException()
    {
        var act = () => new PricingRule(null!, 50);

        act.Should().Throw<InvalidPricingRuleException>()
            .WithMessage("*SKU cannot be null or empty*");
    }

    [Fact]
    public void Constructor_WhenSkuIsEmpty_ThrowsInvalidPricingRuleException()
    {
        var act = () => new PricingRule("", 50);

        act.Should().Throw<InvalidPricingRuleException>()
            .WithMessage("*SKU cannot be null or empty*");
    }

    [Fact]
    public void Constructor_WhenUnitPriceIsZero_ThrowsInvalidPricingRuleException()
    {
        var act = () => new PricingRule("A", 0);

        act.Should().Throw<InvalidPricingRuleException>()
            .WithMessage("*Unit price must be greater than zero*");
    }

    [Fact]
    public void Constructor_WhenUnitPriceIsNegative_ThrowsInvalidPricingRuleException()
    {
        var act = () => new PricingRule("A", -10);

        act.Should().Throw<InvalidPricingRuleException>()
            .WithMessage("*Unit price must be greater than zero*");
    }

    [Fact]
    public void Constructor_WhenSpecialQuantityProvidedWithoutSpecialPrice_ThrowsInvalidPricingRuleException()
    {
        var act = () => new PricingRule("A", 50, specialQuantity: 3, specialPrice: null);

        act.Should().Throw<InvalidPricingRuleException>()
            .WithMessage("*Special quantity and special price must both be provided*");
    }

    [Fact]
    public void Constructor_WhenSpecialPriceProvidedWithoutSpecialQuantity_ThrowsInvalidPricingRuleException()
    {
        var act = () => new PricingRule("A", 50, specialQuantity: null, specialPrice: 130);

        act.Should().Throw<InvalidPricingRuleException>()
            .WithMessage("*Special quantity and special price must both be provided*");
    }

    [Fact]
    public void Constructor_WhenSpecialQuantityIsZero_ThrowsInvalidPricingRuleException()
    {
        var act = () => new PricingRule("A", 50, specialQuantity: 0, specialPrice: 130);

        act.Should().Throw<InvalidPricingRuleException>()
            .WithMessage("*Special quantity must be greater than zero*");
    }

    [Fact]
    public void Constructor_WhenSpecialQuantityIsNegative_ThrowsInvalidPricingRuleException()
    {
        var act = () => new PricingRule("A", 50, specialQuantity: -3, specialPrice: 130);

        act.Should().Throw<InvalidPricingRuleException>()
            .WithMessage("*Special quantity must be greater than zero*");
    }

    [Fact]
    public void Constructor_WhenSpecialPriceIsZero_ThrowsInvalidPricingRuleException()
    {
        var act = () => new PricingRule("A", 50, specialQuantity: 3, specialPrice: 0);

        act.Should().Throw<InvalidPricingRuleException>()
            .WithMessage("*Special price must be greater than zero*");
    }

    [Fact]
    public void Constructor_WhenSpecialPriceIsNegative_ThrowsInvalidPricingRuleException()
    {
        var act = () => new PricingRule("A", 50, specialQuantity: 3, specialPrice: -130);

        act.Should().Throw<InvalidPricingRuleException>()
            .WithMessage("*Special price must be greater than zero*");
    }

    [Fact]
    public void Constructor_WhenSpecialPriceNotADiscount_ThrowsInvalidPricingRuleException()
    {
        var act = () => new PricingRule("A", 50, specialQuantity: 3, specialPrice: 200);

        act.Should().Throw<InvalidPricingRuleException>()
            .WithMessage("*Special price*must be less than*regular price*");
    }
}