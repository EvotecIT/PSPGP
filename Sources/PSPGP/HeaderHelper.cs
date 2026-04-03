using System.Collections;
using System.Collections.Generic;

namespace PSPGP;

internal static class HeaderHelper {
    internal static Dictionary<string, string> ToDictionary(Hashtable headers) {
        if (headers == null || headers.Count == 0) {
            return null;
        }

        var dictionary = new Dictionary<string, string>();
        foreach (DictionaryEntry entry in headers) {
            string key = entry.Key?.ToString();
            if (string.IsNullOrEmpty(key)) {
                continue;
            }

            dictionary[key] = entry.Value?.ToString() ?? string.Empty;
        }

        return dictionary;
    }
}
