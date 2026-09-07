using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SimpleFileServer.Web.Utils;

internal static class SplitCookieHandler
{
    private const int SafeCookieSizeLimit = 3500;

    public static Dictionary<string, string> ToSplitCookies(
        string namePrefix,
        string value)
    {
        // Convert to base 64 to make them non-plain text
        string base64Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

        var dictionary = new Dictionary<string, string>();
        int partIndex = 0;
        foreach (char[] chunk in base64Value.Chunk(SafeCookieSizeLimit))
        {
            dictionary.Add($"{namePrefix}_{partIndex}", new string(chunk));
        }

        dictionary.Add($"{namePrefix}_length", dictionary.Count.ToString());

        return dictionary;
    }

    public static bool TryGetSplitCookieValue(IRequestCookieCollection requestCookies, string namePrefix, [NotNullWhen(true)] out string? value)
    {
        value = null;

        if (!requestCookies.TryGetValue($"{namePrefix}_length", out string? rawLength)
            || !int.TryParse(rawLength, out int length))
        {
            return false;
        }

        var valueBuilder = new StringBuilder();
        for (int partIndex = 0; partIndex < length; partIndex++)
        {
            if (!requestCookies.TryGetValue($"{namePrefix}_{partIndex}", out string? partValue))
            {
                // Some part is missing
                return false;
            }

            valueBuilder.Append(partValue);
        }

        value = Encoding.UTF8.GetString(Convert.FromBase64String(valueBuilder.ToString()));
        return true;
    }
}
