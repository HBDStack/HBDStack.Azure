using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderExtensions
{
    #region Methods

    public static IOptions<TConfig> GetConfigure<TConfig>(this IServiceProvider provider) where TConfig : class, new()
        => provider.GetService<IOptions<TConfig>>();

    public static IOptions<TConfig> GetRequiredConfigure<TConfig>(this IServiceProvider provider) where TConfig : class, new()
        => provider.GetRequiredService<IOptions<TConfig>>();

    #endregion Methods
}