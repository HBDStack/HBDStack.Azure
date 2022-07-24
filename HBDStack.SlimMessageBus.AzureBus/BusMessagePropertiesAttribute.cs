namespace HBDStack.SlimMessageBus.AzureBus;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class BusMessagePropertiesAttribute : Attribute
{
    public BusMessagePropertiesAttribute(params string[] properties) => Properties = properties;

    public string[] Properties { get; set; }
}