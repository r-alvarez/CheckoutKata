# Checkout Kata - TDD Implementation

A supermarket checkout system built with Test-Driven Development. This was created as part of a technical interview to demonstrate clean code, SOLID principles, and modern C# practices.

**Kata Source:** [Code Kata - Back to the Checkout](https://github.com/brighthr/checkout-kata)

---

## What Does It Do?

- Scans items and calculates totals
- Handles multi-buy offers (like "3 for 130")
- Supports different pricing strategies (multi-buy, BOGOF, percentage discounts)
- Items can be scanned in any order
- Real-time price calculation

---

## How It's Organized

I've used **vertical slice architecture** - organizing by feature rather than technical layer. Each domain (Checkout, Pricing) contains everything it needs:

```folder
Checkout/
  Contracts/
  CheckoutService.cs

Pricing/
  Contracts/
  Models/
  Strategies/
  PricingService.cs
```

This made sense for a class library of this size. It keeps related code together and makes it easier to find things - "Where's the pricing logic?" Just look in `Pricing/`.

---

## Design Decisions

### Strategy Pattern for Pricing

Different pricing algorithms need to be swappable. I implemented:

- `MultiBuyPricingStrategy` - Buy N items for a special price
- `BuyOneGetOneFreeStrategy` - BOGOF offers
- `PercentageDiscountStrategy` - Percentage off when you buy enough

This follows Open/Closed - I can add new pricing types without touching existing code. Just implement `IPricingStrategy` and you're done.

### Factory Pattern

The `PricingStrategyFactory` creates and caches strategies:

```csharp
public IPricingStrategy GetStrategy(PricingStrategyType type) => type switch
{
    PricingStrategyType.MultiBuy => _strategies[type],
    // ... other strategies
};
```

Strategies are stateless, so I cache them as singletons. O(1) lookup.

### Dependency Injection

Everything is wired up through DI:

```csharp
services.AddCheckoutServices(pricingRules, PricingStrategyType.MultiBuy);
```

This keeps things loosely coupled and testable. The checkout service is scoped (new instance per request), but strategies and services are singletons (stateless).

---

## Testing Approach

I followed strict TDD throughout:

1. Write a failing test (RED)
2. Write minimal code to pass (GREEN)
3. Refactor for quality

**Current Coverage:**

- 22 checkout tests covering all kata requirements
- 13 pricing rule validation tests
- 5 factory tests
- 5 BOGOF strategy tests
- 4 percentage discount tests
- 7 integration tests verifying DI setup

**Total: 56 tests**

All tests use:

- AAA pattern (Arrange-Act-Assert)
- Interface-first testing (tests depend on `ICheckout`, not `CheckoutService`)
- FluentAssertions for readable assertions

---

## Performance Notes

| Operation | Complexity | Why |
|-----------|------------|-----|
| `Scan(item)` | O(1) | Dictionary insert/update |
| `GetTotalPrice()` | O(n) | n = unique SKUs, not total items |
| Multi-buy calculation | O(1) | Division-based, no loops |
| Strategy lookup | O(1) | Dictionary with caching |

Example: Scanning 1000 items of 5 different SKUs = 5 price calculations, not 1000.

---

## Key Choices Explained

### Dictionary for Item Tracking

I use `Dictionary<string, int>` to track scanned items. Considered `List<string>` but that would be O(n) for counting. Dictionary gives O(1) lookup and is a natural fit for counting.

### Records for Models

`PricingRule` is a record, not a class:

- Immutable by default
- Less boilerplate (primary constructor)
- Value-based equality (useful for testing)
- Signals intent: "this is data"

### Pattern Matching

I use modern C# pattern matching throughout:

```csharp
if (specialQuantity is <= 0)  // Instead of: specialQuantity.HasValue && specialQuantity.Value <= 0
```

More concise and readable while showing knowledge of modern C# features.

### Application-Level Exceptions

Exceptions live in `/Exceptions/` rather than feature folders. `InvalidSkuException` could be thrown from either Checkout or Pricing, so keeping them at the application level avoids duplication.

### Validation in Constructor

`PricingRule` validates itself in the constructor. This is Domain-Driven Design - value objects enforce their own invariants. Makes it impossible to create an invalid pricing rule (fail-fast principle).

For complex validation or API inputs, I'd use FluentValidation instead.

---

## Dependencies

**Application:**

- Microsoft.Extensions.DependencyInjection.Abstractions

**Tests:**

- FluentAssertions
- Microsoft.Extensions.DependencyInjection
- xUnit

Kept dependencies minimal - only what's actually needed.

---

## Usage Example

```csharp
var rules = new PricingRule[]
{
    new("A", unitPrice: 50, specialQuantity: 3, specialPrice: 130),
    new("B", unitPrice: 30, specialQuantity: 2, specialPrice: 45),
    new("C", unitPrice: 20),
    new("D", unitPrice: 15)
};

var services = new ServiceCollection();
services.AddCheckoutServices(rules, PricingStrategyType.MultiBuy);
var provider = services.BuildServiceProvider();

var checkout = provider.GetRequiredService<ICheckout>();

checkout.Scan("A");
checkout.Scan("A");
checkout.Scan("A");  // 3 A's = 130 (special offer)
checkout.Scan("B");
checkout.Scan("B");  // 2 B's = 45 (special offer)

var total = checkout.GetTotalPrice();  // 175
```

To use a different strategy, just change the strategy type:

```csharp
services.AddCheckoutServices(rules, PricingStrategyType.BuyOneGetOneFree);
```

---

## Running Tests

```bash
# From solution root
dotnet test

# With detailed output
dotnet test --verbosity normal

# Specific test class
dotnet test --filter "CheckoutTests"
```

---

## Kata Requirements

All 14 original kata test cases pass, plus comprehensive validation and integration tests:

- Empty checkout returns 0
- Single item returns unit price
- Multiple items sum correctly
- Special offers apply (AAA, AAAA, AAABB, etc.)
- Items can be scanned in any order (DABABA test)
- Incremental price calculation (call GetTotalPrice() multiple times)
- Flexible pricing rules (strategy pattern + factory)
- Exception handling (unknown SKUs, invalid rules)

---

## Why These Choices?

This kata is small enough to "just code," but I treated it like production code to demonstrate:

- Architectural thinking (vertical slices, bounded contexts)
- Design pattern knowledge (Strategy, Factory, DI)
- SOLID principles in practice
- TDD discipline
- Modern C# proficiency
- Production-ready code structure

The goal was to show how I'd approach a real system, not just pass the kata tests.

---

## Acknowledgements

Kata adapted from [Code Kata - Back to the Checkout](http://codekata.com/kata/kata09-back-to-the-checkout/)

Built as part of a technical interview process.
