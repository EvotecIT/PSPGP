using System;
using System.IO;
using System.Text;

namespace PSPGP;

internal static class KeyMaterialHelper {
    private const string BeginMarker = "-----BEGIN PGP ";
    private const string EndMarker = "-----END PGP ";

    internal static Stream OpenRead(string filePath) {
        byte[] bytes = File.ReadAllBytes(filePath);
        return new MemoryStream(Normalize(bytes), writable: false);
    }

    internal static byte[] Normalize(byte[] bytes) {
        if (TryNormalizeArmoredKey(bytes, out byte[] normalized)) {
            return normalized;
        }

        return bytes;
    }

    private static bool TryNormalizeArmoredKey(byte[] bytes, out byte[] normalized) {
        string text = DecodeText(bytes);
        int begin = text.IndexOf(BeginMarker, StringComparison.Ordinal);
        if (begin < 0) {
            normalized = null;
            return false;
        }

        int end = text.LastIndexOf(EndMarker, StringComparison.Ordinal);
        if (end < begin) {
            normalized = null;
            return false;
        }

        int endOfLine = text.IndexOfAny(new[] { '\r', '\n' }, end);
        string block = (endOfLine >= 0 ? text.Substring(begin, endOfLine - begin) : text.Substring(begin)).Trim();
        normalized = Encoding.UTF8.GetBytes(block);
        return true;
    }

    private static string DecodeText(byte[] bytes) {
        if (HasPrefix(bytes, 0xEF, 0xBB, 0xBF)) {
            return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
        }

        if (HasPrefix(bytes, 0xFF, 0xFE)) {
            return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
        }

        if (HasPrefix(bytes, 0xFE, 0xFF)) {
            return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);
        }

        return Encoding.UTF8.GetString(bytes);
    }

    private static bool HasPrefix(byte[] bytes, params byte[] prefix) {
        if (bytes.Length < prefix.Length) {
            return false;
        }

        for (int i = 0; i < prefix.Length; i++) {
            if (bytes[i] != prefix[i]) {
                return false;
            }
        }

        return true;
    }
}
