using Refit;
using System.Threading.Tasks;

namespace ConsoleApp;

public interface IDeepSeaApi
{
    #region Methods

    [Get("/v1/Country")]
    public Task<string> GetCountries();

    #endregion Methods
}