using Refit;
using System.Threading.Tasks;

namespace HBDStack.AzProxy.Core.Tests;

public interface IDeepSeaApi
{
    [Post("/v1/References/rates")]
    public Task<string> Post([Body]Model model);
}