using HBDStack.AzProxy.Core.Providers;
using HBDStack.AzProxy.Apim.Credential;
using HBDStack.AzProxy.Apim.Handlers;

namespace HBDStack.AzProxy.Apim.Providers;

public sealed class ApimTokenClientHandler : TokenProvider
{
    #region Fields

    private readonly ApimCredential credential;

    #endregion Fields

    #region Constructors

    public ApimTokenClientHandler(ApimCredential credential) => this.credential = credential;

    #endregion Constructors

    #region Methods

    protected override ValueTask<string> GetTokenAsync(CancellationToken cancellationToken = default) => new(SasTokenCreator.NewSas(credential));

    #endregion Methods
}