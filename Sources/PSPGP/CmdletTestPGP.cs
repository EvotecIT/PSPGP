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

    /// <summary>Destination folder for verification reports.</summary>
    [Parameter(ParameterSetName = "Folder")]
    public string OutputFolderPath { get; set; }

    /// <summary>File path to verify.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "File")]
    public string FilePath { get; set; }

    /// <summary>Output path for verification result.</summary>
    [Parameter(ParameterSetName = "File")]
    public string OutFilePath { get; set; }

    /// <summary>Encrypted text to verify.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "String")]
    public string String { get; set; }

    /// <summary>Throws when encrypted content is passed to verify methods.</summary>
    [Parameter]
    public SwitchParameter ThrowIfEncrypted { get; set; }

    /// <summary>Verifies clear-signed content instead of detached/regular signatures.</summary>
    [Parameter]
    public SwitchParameter ClearSigned { get; set; }

    /// <summary>
    /// Validates signatures for files, folders or strings
    /// using the provided public keys.
    /// </summary>
    protected override void ProcessRecord() {
        try {
            var publicKeys = new List<string>();
            foreach (var path in FilePathPublic) {
                var resolved = PathResolver.Resolve(this, path);
                if (!File.Exists(resolved)) {
                    ErrorActionHelper.WriteErrorOrWarning(
                        this,
                        new FileNotFoundException($"Public key doesn't exist {resolved}"),
                        "PublicKeyNotFound",
                        ErrorCategory.InvalidArgument,
                        resolved,
                        $"Public key doesn't exist {resolved}");
                    return;
                }
                DateTime? expiration = KeyExpirationHelper.GetExpiration(resolved);
                KeyExpirationHelper.WarnIfExpired(this, resolved, expiration);
                publicKeys.Add(resolved);
            }

            if (ParameterSetName == "Folder") {
                string resolvedFolder = PathResolver.Resolve(this, FolderPath);
                foreach (var file in Directory.GetFiles(resolvedFolder, "*", SearchOption.AllDirectories)) {
                    bool status = false;
                    string error = string.Empty;
                    string signer = null;
                    foreach (var key in publicKeys) {
                        try {
                            using var publicKeyStream = KeyMaterialHelper.OpenRead(key);
                            var encryptionKeys = new EncryptionKeys(publicKeyStream);
                            var pgp = new PGP(encryptionKeys);
                            status = ClearSigned.IsPresent
                                ? pgp.VerifyClearFile(new FileInfo(file))
                                : pgp.VerifyFile(new FileInfo(file), ThrowIfEncrypted.IsPresent);
                            if (status) {
                                signer = key;
                                break;
                            }
                        } catch (Exception ex) {
                            error = PgpExceptionHelper.Normalize(ex, key).Message;
                        }
                    }
                    var result = new VerificationResult {
                        FilePath = file,
                        Status = status,
                        Error = status ? null : error,
                        Signer = signer
                    };
                    WriteObject(result);
                }
            } else if (ParameterSetName == "File") {
                string resolvedFile = PathResolver.Resolve(this, FilePath);
                bool status = false;
                string error = string.Empty;
                string signer = null;
                foreach (var key in publicKeys) {
                    try {
                        using var publicKeyStream = KeyMaterialHelper.OpenRead(key);
                        var encryptionKeys = new EncryptionKeys(publicKeyStream);
                        var pgp = new PGP(encryptionKeys);
                        status = ClearSigned.IsPresent
                            ? pgp.VerifyClearFile(new FileInfo(resolvedFile))
                            : pgp.VerifyFile(new FileInfo(resolvedFile), ThrowIfEncrypted.IsPresent);
                        if (status) {
                            signer = key;
                            break;
                        }
                    } catch (Exception ex) {
                        error = PgpExceptionHelper.Normalize(ex, key).Message;
                    }
                }
                var result = new VerificationResult {
                    FilePath = resolvedFile,
                    Status = status,
                    Error = status ? null : error,
                    Signer = signer
                };
                WriteObject(result);
            } else if (ParameterSetName == "String") {
                bool status = false;
                string error = string.Empty;
                string signer = null;
                foreach (var key in publicKeys) {
                    try {
                        using var publicKeyStream = KeyMaterialHelper.OpenRead(key);
                        var encryptionKeys = new EncryptionKeys(publicKeyStream);
                        var pgp = new PGP(encryptionKeys);
                        if (ClearSigned.IsPresent) {
                            pgp.VerifyClearArmoredString(String);
                        } else {
                            pgp.VerifyArmoredString(String, ThrowIfEncrypted.IsPresent);
                        }
                        status = true;
                        signer = key;
                        break;
                    } catch (Exception ex) {
                        error = PgpExceptionHelper.Normalize(ex, key).Message;
                    }
                }
                var result = new VerificationResult {
                    Status = status,
                    Error = status ? null : error,
                    Signer = signer
                };
                WriteObject(result);
            }
        } catch (Exception ex) {
            WriteError(new ErrorRecord(ex, "TestPGPFailed", ErrorCategory.NotSpecified, null));
        }
    }
}
