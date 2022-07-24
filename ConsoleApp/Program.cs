using HBDStack.AzProxy.Core;
using System;

namespace ConsoleApp;

internal class Program
{
    #region Methods

    private static async System.Threading.Tasks.Task Main(string[] args)
    {
        var api = AzProxyFactory.For<IDeepSeaApi>(new Uri("https://sg-dev-deepsea-api.azurewebsites.net"),
            builder => builder.UseClientCert("dev-root-vault.pfx"));

        var apim = AzProxyFactory.For<IDeepSeaApi>(new Uri("https://api.transwap.dev/dev-deepsea"),
            builder => builder
                .UseClientCert("dev-root-vault.pfx")
                .AddHeaderValue("transwap-api-key", "D-XhIoNC/g/<>|#LIr53t615RFY*?SD3ytECqH&%pns5pCItiw"));

        try
        {
            Console.WriteLine("Direct call.");
            var result1 = await api.GetCountries();

            Console.WriteLine(result1);
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        try
        {
            Console.WriteLine("Apim call.");
            var result2 = await apim.GetCountries();

            Console.WriteLine(result2);
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
        Console.ReadKey();
    }

    #endregion Methods
}