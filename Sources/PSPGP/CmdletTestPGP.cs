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
[Cmdlet(VerbsDiagnostic.Test, "PGP", DefaultParameterSetName = "File", SupportsShouldProcess = true)]
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
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "File")]
    [Alias("FullName", "LiteralPath")]
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

    /// <summary>Retained for compatibility. Encrypted input is always rejected; use Unprotect-PGP -Verify.</summary>
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
                string root = PathResolver.Resolve(this, FolderPath);
                string destination = string.IsNullOrEmpty(OutputFolderPath) ? null : PathResolver.Resolve(this, OutputFolderPath);
                if (destination == null) {
                    foreach (string file in FileWorkflow.EnumerateFiles(root))
                        WriteObject(VerifyFileWithAnyKey(file, null, null, publicKeys));
                } else {
                    foreach (var item in FileWorkflow.Plan(root, destination, FileWorkflow.RemoveSignedSuffix))
                        WriteObject(VerifyFileWithAnyKey(item.Input, null, item.Output, publicKeys));
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
        } catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
            WriteError(PgpExceptionHelper.CreateErrorRecord(ex, "TestPGPFailed"));
        }
    }

    private List<string> ResolvePublicKeys() {
        var publicKeys = new List<string>();
        foreach (string path in FilePathPublic) {
            string resolved = PathResolver.Resolve(this, path);
            if (!File.Exists(resolved)) {
                CmdletError.Write(
                    this,
                    new FileNotFoundException($"Public key doesn't exist {resolved}"),
                    "PublicKeyNotFound",
                    ErrorCategory.InvalidArgument,
                    resolved);
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
        string error = "No valid signature matched the supplied public keys.";
        string signer = null;
        string verifiedOutput = null;
        string approvedOutput = string.IsNullOrEmpty(outputPath) || ShouldProcess(outputPath, "Write verified content") ? outputPath : null;

        foreach (string key in publicKeys) {
            try {
                using var publicKeyStream = KeyMaterialHelper.OpenRead(key);
                var encryptionKeys = new EncryptionKeys(publicKeyStream);
                var pgp = new PGP(encryptionKeys);
                status = !string.IsNullOrEmpty(signaturePath)
                    ? pgp.VerifyDetached(new FileInfo(filePath), new FileInfo(signaturePath))
                    : !string.IsNullOrEmpty(approvedOutput)
                        ? VerifyFileToOutput(pgp, filePath, approvedOutput)
                        : ClearSigned.IsPresent
                            ? pgp.VerifyClearFile(new FileInfo(filePath))
                            : pgp.VerifyFile(new FileInfo(filePath), ThrowIfEncrypted.IsPresent);
                if (status) {
                    signer = key;
                    verifiedOutput = string.IsNullOrEmpty(signaturePath) ? approvedOutput : null;
                    break;
                }
            } catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
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
        string error = "No valid signature matched the supplied public keys.";
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
            } catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
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
        FileWorkflow.EnsureOutputDirectory(inputFile, outputFile);
        return ClearSigned.IsPresent
            ? pgp.VerifyClear(new FileInfo(inputFile), new FileInfo(outputFile))
            : pgp.Verify(new FileInfo(inputFile), new FileInfo(outputFile), true);
    }
}
