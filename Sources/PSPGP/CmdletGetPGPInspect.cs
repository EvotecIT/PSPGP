using System;
using System.IO;
using System.Management.Automation;
using System.Text;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PgpCore;
using PgpCore.Models;

namespace PSPGP;

/// <summary>
/// Inspects PGP content and returns message metadata.
/// </summary>
/// <example>
/// <code>
/// $Signature = Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'secret' -String 'Signed text'
/// Get-PGPInspect -String $Signature
/// </code>
/// </example>
[Cmdlet(VerbsCommon.Get, "PGPInspect", DefaultParameterSetName = "File")]
[OutputType(typeof(PGPInspectInfo))]
public class CmdletGetPGPInspect : PSCmdlet {
    /// <summary>Folder containing PGP files to inspect.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Folder")]
    public string FolderPath { get; set; }

    /// <summary>File to inspect.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "File")]
    public string FilePath { get; set; }

    /// <summary>Armored message content to inspect.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "String")]
    public string String { get; set; }

    /// <summary>
    /// Inspects the selected input and writes message metadata.
    /// </summary>
    protected override void ProcessRecord() {
        var pgp = new PGP();
        try {
            if (ParameterSetName == "Folder") {
                string resolvedFolder = PathResolver.Resolve(this, FolderPath);
                foreach (string file in Directory.GetFiles(resolvedFolder, "*", SearchOption.AllDirectories)) {
                    try {
                        FileInfo fileInfo = new(file);
                        WriteObject(ToInfo(file, pgp.Inspect(fileInfo), TryGetIntegrityProtected(fileInfo)));
                    } catch (System.Exception ex) {
                        WriteError(new ErrorRecord(NormalizeInspectException(ex), "GetPGPInspectFailed", ErrorCategory.NotSpecified, file));
                    }
                }
            } else if (ParameterSetName == "File") {
                string resolvedFile = PathResolver.Resolve(this, FilePath);
                FileInfo fileInfo = new(resolvedFile);
                WriteObject(ToInfo(resolvedFile, pgp.Inspect(fileInfo), TryGetIntegrityProtected(fileInfo)));
            } else {
                WriteObject(ToInfo(null, pgp.Inspect(String), TryGetIntegrityProtected(String)));
            }
        } catch (System.Exception ex) {
            WriteError(new ErrorRecord(NormalizeInspectException(ex), "GetPGPInspectFailed", ErrorCategory.NotSpecified, null));
        }
    }

    private static System.Exception NormalizeInspectException(System.Exception exception) {
        if (exception is System.NullReferenceException) {
            return new System.NotSupportedException(
                "PgpCore inspect could not read this message. Signed, armored, and encrypted packet metadata may still be inspectable for other inputs.",
                exception);
        }

        return exception;
    }

    private static bool? TryGetIntegrityProtected(FileInfo fileInfo) {
        try {
            using FileStream stream = fileInfo.OpenRead();
            return TryGetIntegrityProtected(stream);
        } catch {
            return null;
        }
    }

    private static bool? TryGetIntegrityProtected(string value) {
        try {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
            return TryGetIntegrityProtected(stream);
        } catch {
            return null;
        }
    }

    private static bool? TryGetIntegrityProtected(Stream stream) {
        try {
            using Stream decoderStream = PgpUtilities.GetDecoderStream(stream);
            var factory = new PgpObjectFactory(decoderStream);
            object pgpObject;
            while ((pgpObject = factory.NextPgpObject()) != null) {
                if (pgpObject is PgpEncryptedDataList encryptedDataList) {
                    foreach (PgpEncryptedData encryptedData in encryptedDataList.GetEncryptedDataObjects()) {
                        return encryptedData.IsIntegrityProtected();
                    }
                }
            }
        } catch {
            return null;
        }

        return null;
    }

    private static string GetHeaderValue(System.Collections.Generic.Dictionary<string, string> messageHeaders, string key) {
        if (messageHeaders != null && messageHeaders.TryGetValue(key, out string value)) {
            return value;
        }

        return null;
    }

    private static PGPInspectInfo ToInfo(string sourcePath, PgpInspectResult result, bool? integrityProtected) {
        return new PGPInspectInfo {
            SourcePath = sourcePath,
            IsArmored = result.IsArmored,
            MessageHeaders = result.MessageHeaders,
            Version = GetHeaderValue(result.MessageHeaders, "Version"),
            Comment = GetHeaderValue(result.MessageHeaders, "Comment"),
            IsCompressed = result.IsCompressed,
            IsEncrypted = result.IsEncrypted,
            IsIntegrityProtected = result.IsIntegrityProtected || integrityProtected == true,
            IsSigned = result.IsSigned,
            SymmetricKeyAlgorithm = result.SymmetricKeyAlgorithm,
            FileName = result.FileName,
            ModificationDateTime = result.ModificationDateTime
        };
    }
}
