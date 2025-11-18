using CheckoutKata.Application.Checkout;
using CheckoutKata.Application.Checkout.Contracts;
using CheckoutKata.Application.Pricing;
using CheckoutKata.Application.Pricing.Contracts;
using CheckoutKata.Application.Pricing.Models;
using CheckoutKata.Application.Pricing.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace CheckoutKata.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCheckoutServices(this IServiceCollection services, IEnumerable<PricingRule> pricingRules)
    {
        // Singleton - Stateless, thread-safe calculation
        services.AddSingleton<IPricingStrategy, MultiBuyPricingStrategy>();

        // Singleton - Stateless service, depends on singleton strategy
        services.AddSingleton<IPricingService, PricingService>();

        // Scoped - Stateful, per-request/session
        services.AddScoped<ICheckout>(sp =>
            new CheckoutService(
                pricingRules,
                sp.GetRequiredService<IPricingService>()));

        return services;
    }
}
