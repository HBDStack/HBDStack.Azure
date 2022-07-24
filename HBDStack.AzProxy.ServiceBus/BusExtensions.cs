using System.Text.Json;
using Azure;
using Azure.Messaging.ServiceBus;
using HBDStack.AzProxy.ServiceBus.Attributes;

namespace HBDStack.AzProxy.ServiceBus;

public static class BusExtensions
{
    // public static Task SendAsync<TMessage>(this IBusMessageSender sender, TMessage message, CancellationToken cancellationToken = default)
    //     where TMessage : class =>
    //     sender.SendAsync(new BusMessage<TMessage>(message), cancellationToken);
    //
    // public static Task SendBatchAsync<TMessage>(this IBusMessageSender sender, IEnumerable<TMessage> messages, CancellationToken cancellationToken = default)
    //     where TMessage : class =>
    //     sender.SendBatchAsync(messages.Select(m => new BusMessage<TMessage>(m)).ToArray(), cancellationToken);

    public static BusMessage<TMessage> ToBusMessage<TMessage>(this ServiceBusReceivedMessage message, JsonSerializerOptions? options = null) where TMessage : class
        => new(message, options);

    internal static void AddRange<T>(this IList<T> list, IEnumerable<T> enumerable)
    {
        foreach (var item in enumerable)
            list.Add(item);
    }

    internal static async Task<IList<T>> ToListAsync<T>(this AsyncPageable<T> pageable, CancellationToken cancellationToken = default) where T : notnull
    {
        var list = new List<T>();
        await foreach (var item in pageable)
        {
            list.Add(item);
            if (cancellationToken.IsCancellationRequested) break;
        }

        return list;
    }

    internal static IDictionary<string, object> GetFilterableProperties<TMessage>(this TMessage message)
    {
        var dic = new Dictionary<string, object>();
        if (message == null) return dic;

        var filterableProps = message.GetType().GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(BusFilterableAttribute), true).Length > 0);

        foreach (var property in filterableProps)
        {
            var name = property.Name;
            var value = property.GetValue(message);

            if (value is null) continue;
            dic.Add(name, value);
        }

        return dic;
    }
}