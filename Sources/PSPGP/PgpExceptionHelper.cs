using PgpCore;
using System;
using System.IO;
using System.Management.Automation;

namespace PSPGP;

internal static class PgpExceptionHelper {
    /// <summary>
    /// Creates a PowerShell error record from a PgpCore or platform exception,
    /// preserving useful key-path guidance and assigning an actionable category.
    /// </summary>
    internal static ErrorRecord CreateErrorRecord(
        Exception exception,
        string errorId,
        object targetObject = null,
        string keyPath = null) {
        Exception normalized = Normalize(exception, keyPath);
        return new ErrorRecord(normalized, errorId, GetErrorCategory(normalized), targetObject);
    }

    /// <summary>
    /// Maps typed PgpCore failures and common platform failures to PowerShell error categories.
    /// Typed PgpCore failures take precedence over generic wrapper exceptions.
    /// </summary>
    internal static ErrorCategory GetErrorCategory(Exception exception) {
        if (ContainsException<IncorrectPassphraseException>(exception)) return ErrorCategory.AuthenticationError;
        if (ContainsException<MessageIntegrityException>(exception)) return ErrorCategory.SecurityError;
        if (ContainsException<InvalidKeyMaterialException>(exception) ||
            ContainsException<NotEncryptedDataException>(exception) ||
            ContainsException<InvalidDataException>(exception)) return ErrorCategory.InvalidData;
        if (ContainsException<MissingKeyException>(exception) ||
            ContainsException<FileNotFoundException>(exception)) return ErrorCategory.ObjectNotFound;
        if (ContainsException<NotSupportedException>(exception)) return ErrorCategory.NotImplemented;
        if (ContainsException<UnauthorizedAccessException>(exception)) return ErrorCategory.PermissionDenied;

        return ErrorCategory.NotSpecified;
    }

    /// <summary>
    /// Adds PSPGP-specific guidance to failures that PgpCore cannot make actionable on its own.
    /// </summary>
    internal static Exception Normalize(Exception exception, string keyPath = null) {
        if (ContainsMessage(exception, "unknown packet type encountered: 20")) {
            return new NotSupportedException(
                "The encrypted content appears to use OpenPGP AEAD (packet type 20), which the underlying PgpCore/BouncyCastle stack cannot decrypt yet. Re-encrypt the file without AEAD, or remove the AEAD preference from the key before creating new encrypted content.",
                exception);
        }

        if (!string.IsNullOrEmpty(keyPath) &&
            (ContainsException<InvalidKeyMaterialException>(exception) ||
             ContainsMessage(exception, "invalid armor header") ||
             ContainsMessage(exception, "unknown object in stream") ||
             ContainsMessage(exception, "Premature end of stream in PartialInputStream"))) {
            return new InvalidDataException(
                $"The PGP key file '{keyPath}' could not be parsed. If it is ASCII-armored, re-export or save it as plain UTF-8 text without BOM or extra wrapper content.",
                exception);
        }

        return exception;
    }

    private static bool ContainsException<TException>(Exception exception) where TException : Exception {
        for (Exception current = exception; current != null; current = current.InnerException) {
            if (current is TException) {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsMessage(Exception exception, string value) {
        for (Exception current = exception; current != null; current = current.InnerException) {
            if ((current.Message ?? string.Empty).IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0) {
                return true;
            }
        }

        return false;
    }
}
