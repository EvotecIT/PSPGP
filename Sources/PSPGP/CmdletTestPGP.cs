using PgpCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace PSPGP;
/// <summary>
/// <para>Verifies PGP signatures for files, folders or strings.</para>
/// </summary>
/// <example>
/// <code>
/// Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $ProtectedString
/// </code>
/// </example>
/// <example>
/// <code>
/// Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FolderPath $PSScriptRoot\Encoded
/// </code>
/// </example>
/// <example>
/// <code>
/// Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePath $PSScriptRoot\Test\Test1.txt -SignaturePath $PSScriptRoot\Test\Test1.txt.sig
/// </code>
/// </example>
/// <example>
/// <code>
/// Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $ClearSigned -ClearSigned
/// </code>
/// </example>
/// <example>
/// <code>
/// Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $ProtectedString -ThrowIfEncrypted
/// </code>
/// </example>
[Cmdlet(VerbsDiagnostic.Test, "PGP", DefaultParameterSetName = "File")]
[OutputType(typeof(VerificationResult))]
public class CmdletTestPGP : PSCmdlet {
    /// <summary>Public key file used to verify signatures.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Folder")]
    [Parameter(Mandatory = true, ParameterSetName = "File")]
    [Parameter(Mandatory = true, ParameterSetName = "String")]
    public string[] FilePathPublic { get; set; }

    /// <summary>Folder containing files to verify.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Folder")]
    public string FolderPath { get; set; }

    /// <summary>Destination folder for verified clear content.</summary>
    [Parameter(ParameterSetName = "Folder")]
    public string OutputFolderPath { get; set; }

    /// <summary>File path to verify.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "File")]
    public string FilePath { get; set; }

    /// <summary>Detached signature file for the input file.</summary>
    [Parameter(ParameterSetName = "File")]
    public string SignaturePath { get; set; }

    /// <summary>Output path for verified clear content.</summary>
    [Parameter(ParameterSetName = "File")]
    public string OutFilePath { get; set; }

    /// <summary>Signed text or original text when Signature is provided.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "String")]
    public string String { get; set; }

    /// <summary>Detached signature for the string input.</summary>
    [Parameter(ParameterSetName = "String")]
    public string Signature { get; set; }

    /// <summary>Throws when encrypted content is passed to verify methods.</summary>
    [Parameter]
    public SwitchParameter ThrowIfEncrypted { get; set; }

    /// <summary>Verifies clear-signed content instead of regular signed content.</summary>
    [Parameter]
    public SwitchParameter ClearSigned { get; set; }

    /// <summary>
    /// Validates signatures for files, folders or strings
    /// using the provided public keys.
    /// </summary>
    protected override void ProcessRecord() {
        try {
            List<string> publicKeys = ResolvePublicKeys();
            if (publicKeys.Count == 0) {
                return;
            }

            if (ParameterSetName == "Folder") {
                string resolvedFolder = PathResolver.Resolve(this, FolderPath);
                string resolvedOutputFolder = !string.IsNullOrEmpty(OutputFolderPath)
                    ? PathResolver.Resolve(this, OutputFolderPath)
                    : null;
                if (!string.IsNullOrEmpty(resolvedOutputFolder)) {
                    Directory.CreateDirectory(resolvedOutputFolder);
                }

                foreach (string file in Directory.GetFiles(resolvedFolder, "*", SearchOption.AllDirectories)) {
                    string outputPath = !string.IsNullOrEmpty(resolvedOutputFolder)
                        ? GetVerifiedOutputPath(resolvedFolder, resolvedOutputFolder, file)
                        : null;
                    WriteObject(VerifyFileWithAnyKey(file, null, outputPath, publicKeys));
                }
            } else if (ParameterSetName == "File") {
                string resolvedFile = PathResolver.Resolve(this, FilePath);
                string resolvedSignature = !string.IsNullOrEmpty(SignaturePath)
                    ? PathResolver.Resolve(this, SignaturePath)
                    : null;
                string resolvedOutput = !string.IsNullOrEmpty(OutFilePath)
                    ? PathResolver.Resolve(this, OutFilePath)
                    : null;
                WriteObject(VerifyFileWithAnyKey(resolvedFile, resolvedSignature, resolvedOutput, publicKeys));
            } else if (ParameterSetName == "String") {
                WriteObject(VerifyStringWithAnyKey(String, Signature, publicKeys));
            }
        } catch (Exception ex) {
            WriteError(PgpExceptionHelper.CreateErrorRecord(ex, "TestPGPFailed"));
        }
    }

    private List<string> ResolvePublicKeys() {
        var publicKeys = new List<string>();
        foreach (string path in FilePathPublic) {
            string resolved = PathResolver.Resolve(this, path);
            if (!File.Exists(resolved)) {
                ErrorActionHelper.WriteErrorOrWarning(
                    this,
                    new FileNotFoundException($"Public key doesn't exist {resolved}"),
                    "PublicKeyNotFound",
                    ErrorCategory.InvalidArgument,
                    resolved,
                    $"Public key doesn't exist {resolved}");
                publicKeys.Clear();
                return publicKeys;
            }
            DateTime? expiration = KeyExpirationHelper.GetExpiration(resolved);
            KeyExpirationHelper.WarnIfExpired(this, resolved, expiration);
            publicKeys.Add(resolved);
        }

        return publicKeys;
    }

    private VerificationResult VerifyFileWithAnyKey(string filePath, string signaturePath, string outputPath, List<string> publicKeys) {
        bool status = false;
        string error = string.Empty;
        string signer = null;
        string verifiedOutput = null;

        foreach (string key in publicKeys) {
            try {
                using var publicKeyStream = KeyMaterialHelper.OpenRead(key);
                var encryptionKeys = new EncryptionKeys(publicKeyStream);
                var pgp = new PGP(encryptionKeys);
                status = !string.IsNullOrEmpty(signaturePath)
                    ? pgp.VerifyDetached(new FileInfo(filePath), new FileInfo(signaturePath))
                    : !string.IsNullOrEmpty(outputPath)
                        ? VerifyFileToOutput(pgp, filePath, outputPath)
                        : ClearSigned.IsPresent
                            ? pgp.VerifyClearFile(new FileInfo(filePath))
                            : pgp.VerifyFile(new FileInfo(filePath), ThrowIfEncrypted.IsPresent);
                if (status) {
                    signer = key;
                    verifiedOutput = string.IsNullOrEmpty(signaturePath) ? outputPath : null;
                    break;
                }
            } catch (Exception ex) {
                error = PgpExceptionHelper.Normalize(ex, key).Message;
            }
        }

        return new VerificationResult {
            FilePath = filePath,
            Status = status,
            Error = status ? null : error,
            Signer = signer,
            OutputPath = status ? verifiedOutput : null
        };
    }

    private VerificationResult VerifyStringWithAnyKey(string input, string signature, List<string> publicKeys) {
        bool status = false;
        string clearText = null;
        string error = string.Empty;
        string signer = null;

        foreach (string key in publicKeys) {
            try {
                using var publicKeyStream = KeyMaterialHelper.OpenRead(key);
                var encryptionKeys = new EncryptionKeys(publicKeyStream);
                var pgp = new PGP(encryptionKeys);
                if (!string.IsNullOrEmpty(signature)) {
                    status = pgp.VerifyDetached(input, signature);
                } else if (ClearSigned.IsPresent) {
                    PgpCore.Models.VerificationResult result = pgp.VerifyAndReadClearArmoredString(input);
                    status = result.IsVerified;
                    clearText = result.ClearText;
                } else {
                    PgpCore.Models.VerificationResult result = pgp.VerifyAndReadSignedArmoredString(input, ThrowIfEncrypted.IsPresent);
                    status = result.IsVerified;
                    clearText = result.ClearText;
                }

                if (status) {
                    signer = key;
                    break;
                }
            } catch (Exception ex) {
                error = PgpExceptionHelper.Normalize(ex, key).Message;
            }
        }

        return new VerificationResult {
            Status = status,
            Error = status ? null : error,
            Signer = signer,
            ClearText = status ? clearText : null
        };
    }

    private bool VerifyFileToOutput(PGP pgp, string inputFile, string outputFile) {
        string temporaryOutput = GetTemporaryOutputFile(outputFile);
        try {
            EnsureDirectoryForFile(temporaryOutput);
            bool verified = ClearSigned.IsPresent
                ? pgp.VerifyClear(new FileInfo(inputFile), new FileInfo(temporaryOutput))
                : pgp.Verify(new FileInfo(inputFile), new FileInfo(temporaryOutput), true);
            if (!verified) {
                return false;
            }

            EnsureDirectoryForFile(outputFile);
            if (File.Exists(outputFile)) {
                File.Delete(outputFile);
            }
            File.Move(temporaryOutput, outputFile);
            return true;
        } finally {
            if (File.Exists(temporaryOutput)) {
                File.Delete(temporaryOutput);
            }
        }
    }

    private static string GetVerifiedOutputPath(string inputFolder, string outputFolder, string inputFile) {
        string relativeInput = GetRelativePath(inputFolder, inputFile);
        string relativeDirectory = Path.GetDirectoryName(relativeInput);
        string outputFileName = GetVerifiedOutputFileName(relativeInput);
        string relativeOutput = string.IsNullOrEmpty(relativeDirectory)
            ? outputFileName
            : Path.Combine(relativeDirectory, outputFileName);

        return Path.Combine(outputFolder, relativeOutput);
    }

    private static string GetVerifiedOutputFileName(string inputFile) {
        string fileName = Path.GetFileName(inputFile);
        foreach (string extension in new[] { ".asc", ".sig", ".pgp", ".gpg" }) {
            if (fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) {
                return fileName.Substring(0, fileName.Length - extension.Length);
            }
        }

        return fileName;
    }

    private static string GetRelativePath(string inputFolder, string inputFile) {
        string root = EnsureTrailingDirectorySeparator(Path.GetFullPath(inputFolder));
        string file = Path.GetFullPath(inputFile);
        var rootUri = new Uri(root);
        var fileUri = new Uri(file);
        if (!string.Equals(rootUri.Scheme, fileUri.Scheme, StringComparison.OrdinalIgnoreCase)) {
            return Path.GetFileName(file);
        }

        string relative = Uri.UnescapeDataString(rootUri.MakeRelativeUri(fileUri).ToString());
        return string.Equals(fileUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase)
            ? relative.Replace('/', Path.DirectorySeparatorChar)
            : relative;
    }

    private static string EnsureTrailingDirectorySeparator(string path) {
        if (path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
            || path.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal)) {
            return path;
        }

        return path + Path.DirectorySeparatorChar;
    }

    private static void EnsureDirectoryForFile(string filePath) {
        string directory = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(directory)) {
            directory = Directory.GetCurrentDirectory();
        }

        Directory.CreateDirectory(directory);
    }

    private static string GetTemporaryOutputFile(string outputFile) {
        string directory = Path.GetDirectoryName(outputFile);
        string fileName = Path.GetFileName(outputFile);
        if (string.IsNullOrEmpty(directory)) {
            directory = Directory.GetCurrentDirectory();
        }

        return Path.Combine(directory, $".{fileName}.{Guid.NewGuid():N}.tmp");
    }
}
