namespace HBDStack.AzProxy.Storage;

public class AzStorageExtensions
{
    internal static string GetAccountKey(string connectionString) => GetValue(connectionString, "AccountKey");

    internal static string GetStoreName(string connectionString) => GetValue(connectionString, "AccountName");
    private static string GetValue(string connectionString, string keyName)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        var found = connectionString.Split(';').FirstOrDefault(s => s.StartsWith(keyName));

        if (found == null)
            throw new ArgumentException("ConnectionString is invalid.", nameof(connectionString));

        return found.Replace($"{keyName}=", string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}