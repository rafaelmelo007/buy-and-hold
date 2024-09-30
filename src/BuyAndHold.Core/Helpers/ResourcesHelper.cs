namespace BuyAndHold.Core.Helpers;
public static class ResourcesHelper
{

    public static string ToString(string path)
    {
        using var stream = typeof(ResourcesHelper).Assembly.
            GetManifestResourceStream("BuyAndHold.Core." + path);
        using var sr = new StreamReader(stream);
        return sr.ReadToEnd();
    }

}

