using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify;

public class Application : IApplication
{
    public string Name
    {
        get => "Shopify";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}