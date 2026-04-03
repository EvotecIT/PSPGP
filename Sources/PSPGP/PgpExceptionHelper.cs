using System;
using System.IO;

namespace PSPGP;

internal static class PgpExceptionHelper {
    internal static Exception Normalize(Exception exception, string keyPath = null) {
        string message = exception?.Message ?? string.Empty;

        if (message.IndexOf("unknown packet type encountered: 20", StringComparison.OrdinalIgnoreCase) >= 0) {
            return new NotSupportedException(
                "The encrypted content appears to use OpenPGP AEAD (packet type 20), which the underlying PgpCore/BouncyCastle stack cannot decrypt yet. Re-encrypt the file without AEAD, or remove the AEAD preference from the key before creating new encrypted content.",
                exception);
        }

        if (!string.IsNullOrEmpty(keyPath) &&
            (message.IndexOf("invalid armor header", StringComparison.OrdinalIgnoreCase) >= 0 ||
             message.IndexOf("unknown object in stream", StringComparison.OrdinalIgnoreCase) >= 0 ||
             message.IndexOf("Premature end of stream in PartialInputStream", StringComparison.OrdinalIgnoreCase) >= 0)) {
            return new InvalidDataException(
                $"The PGP key file '{keyPath}' could not be parsed. If it is ASCII-armored, re-export or save it as plain UTF-8 text without BOM or extra wrapper content.",
                exception);
        }

        return exception;
    }
}
