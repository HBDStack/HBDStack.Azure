using SlimMessageBus;

namespace HBDStack.SlimMessageBus.AzureBus;

public interface IQueueOptions
{
    IQueueOptions Produce<T>();
    void Consume<TMessage, THandler>(int instances = 1) where THandler : IConsumer<TMessage>;
    void Consume<THandler>(int instances = 1);
}

public interface ITopicOptions
{
    ITopicOptions Produce<T>();
    ISubscriptionOptions Subscription(string name);
}

public interface ISubscriptionOptions
{
    ISubscriptionOptions SubscriptionSqlFilter(string sqlFilter = "1=1", string? name = "default");
    void Consume<TMessage, THandler>(int instances = 1) where THandler : IConsumer<TMessage>;
    void Consume<THandler>(int instances = 1);
}

public interface IBusOptions
{
    ITopicOptions Topic(string name);
    IQueueOptions Queue(string name);
}

internal class SubscriptionOptions : ISubscriptionOptions
{
    internal readonly string Name;
    internal string? SqlFilter;
    internal string? SqlFilterName;
    private readonly TopicOptions topicOptions;
    internal Type? ConsumerHandlerType;
    internal Type? ConsumerMessageType;
    internal int Instances;

    public SubscriptionOptions(string name, TopicOptions topicOptions)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        this.topicOptions = topicOptions;
    }

    public ISubscriptionOptions SubscriptionSqlFilter(string sqlFilter, string? name = "default")
    {
        SqlFilter = sqlFilter;
        SqlFilterName = name;
        return this;
    }

    public void Consume<TMessage, THandler>(int instances = 1) where THandler : IConsumer<TMessage>
    {
        if (ConsumerHandlerType != null) throw new InvalidOperationException($"{nameof(ConsumerHandlerType)} is already defined.");
        ConsumerHandlerType = typeof(THandler);
        ConsumerMessageType = typeof(TMessage);
        Instances = instances;
    }

    public void Consume<THandler>(int instances = 1)
    {
        if (ConsumerHandlerType != null) throw new InvalidOperationException($"{nameof(ConsumerHandlerType)} is already defined.");
        ConsumerHandlerType = typeof(THandler);
        ConsumerMessageType = topicOptions.ProduceType ?? throw new InvalidOperationException($"{topicOptions.ProduceType} is not provided. Use Consume<TMessage, THandler> method instead.");
        Instances = instances;
    }
}

internal class QueueOptions : IQueueOptions
{
    internal readonly string Name;
    internal Type? ConsumerHandlerType;
    internal Type? ConsumerMessageType;
    internal int Instances;
    internal Type? ProduceType;
    public QueueOptions(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

    public IQueueOptions Produce<T>()
    {
        if (ProduceType != null) throw new InvalidOperationException($"{nameof(ProduceType)} is already defined.");
        ProduceType = typeof(T);
        return this;
    }

    public void Consume<TMessage, THandler>(int instances = 1) where THandler : IConsumer<TMessage>
    {
        if (ConsumerHandlerType != null) throw new InvalidOperationException($"{nameof(ConsumerHandlerType)} is already defined.");
        ConsumerHandlerType = typeof(THandler);
        ConsumerMessageType = typeof(TMessage);
        Instances = instances;
    }

    public void Consume<THandler>(int instances = 1)
    {
        if (ConsumerHandlerType != null) throw new InvalidOperationException($"{nameof(ConsumerHandlerType)} is already defined.");
        ConsumerHandlerType = typeof(THandler);
        ConsumerMessageType = ProduceType ?? throw new InvalidOperationException($"{ProduceType} is not provided. Use Consume<TMessage, THandler> method instead.");
        Instances = instances;
    }
}

internal class TopicOptions : ITopicOptions
{
    internal readonly string Name;
    internal Type? ProduceType;
    internal readonly Dictionary<string, SubscriptionOptions> Subscriptions = new();

    public TopicOptions(string name) => Name = name;

    public ITopicOptions Produce<T>()
    {
        if (ProduceType != null) throw new InvalidOperationException($"{nameof(ProduceType)} is already defined.");
        ProduceType = typeof(T);
        return this;
    }

    public ISubscriptionOptions Subscription(string name)
    {
        var sub = new SubscriptionOptions(name, this);
        Subscriptions.Add(name, sub);
        return sub;
    }
}

internal class BusOptions : IBusOptions
{
    internal readonly Dictionary<string, TopicOptions> Topics = new();
    internal readonly Dictionary<string, QueueOptions> Queues = new();

    public ITopicOptions Topic(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        return Topics.ContainsKey(name) ? Topics[name] : Topics[name] = new TopicOptions(name);
    }

    public IQueueOptions Queue(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        var op = new QueueOptions(name);
        Queues.Add(name, op);
        return op;
    }
}