using System.Text;

namespace BuyAndHold.Core.Extensions;
public static class StringExtensions
{
    public static string? ToBase64(this string content)
    {
        byte[] contentBytes = Encoding.UTF8.GetBytes(content);
        string base64Content = Convert.ToBase64String(contentBytes);
        return base64Content;
    }

    public static string? ToJson(this object obj)
    {
        if (obj is null) return default;
        return JsonSerializer.Serialize(obj);
    }

    public static string? NullIfEmpty(this string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;

        return text;
    }


}
