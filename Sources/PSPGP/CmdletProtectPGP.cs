using Org.BouncyCastle.Bcpg;
using PgpCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace PSPGP;
/// <summary>
/// <para>
/// Encrypts or signs files, folders or strings using one or more public keys.
/// Use <c>-SignOnly</c> or the <c>Sign*</c> parameter sets to create detached
/// signatures without encryption.
/// </para>
/// <example>
/// <code>
/// Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -FolderPath $PSScriptRoot\Test -OutputFolderPath $PSScriptRoot\Encoded
/// </code>
/// </example>
/// <example>
/// <code>
/// Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -FilePath $PSScriptRoot\Test\Test1.txt -OutFilePath $PSScriptRoot\Encoded\Test1.txt.pgp
/// </code>
/// </example>
/// <example>
/// <code>
/// Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -String "Sensitive text"
/// </code>
/// </example>
/// <example>
/// <code>
/// Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP.asc -SignPassword 'secret' -String "Signed content"
/// </code>
/// </example>
/// </summary>
[Cmdlet("Protect", "PGP", DefaultParameterSetName = "File")]
public class CmdletProtectPGP : PSCmdlet {
    /// <summary>Public key files used for encryption.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Folder")]
    [Parameter(Mandatory = true, ParameterSetName = "File")]
    [Parameter(Mandatory = true, ParameterSetName = "String")]
    [Parameter(ParameterSetName = "SignFolder")]
    [Parameter(ParameterSetName = "SignFile")]
    [Parameter(ParameterSetName = "SignString")]
    public string[] FilePathPublic { get; set; }

    /// <summary>Folder to encrypt when using the Folder parameter set.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Folder")]
    [Parameter(Mandatory = true, ParameterSetName = "SignFolder")]
    public string FolderPath { get; set; }

    /// <summary>Destination folder for encrypted files.</summary>
    [Parameter(ParameterSetName = "Folder")]
    [Parameter(ParameterSetName = "SignFolder")]
    public string OutputFolderPath { get; set; }

    /// <summary>File to encrypt when using the File parameter set.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "File")]
    [Parameter(Mandatory = true, ParameterSetName = "SignFile")]
    public string FilePath { get; set; }

    /// <summary>Output file path for the encrypted file.</summary>
    [Parameter(ParameterSetName = "File")]
    [Parameter(ParameterSetName = "SignFile")]
    public string OutFilePath { get; set; }

    /// <summary>String content to encrypt.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "String")]
    [Parameter(Mandatory = true, ParameterSetName = "SignString")]
    public string String { get; set; }

    /// <summary>
    /// Private key used for signing data. Mandatory when using the
    /// <c>Sign*</c> parameter sets or the <c>-SignOnly</c> switch.
    /// </summary>
    [Parameter(ParameterSetName = "Folder")]
    [Parameter(ParameterSetName = "File")]
    [Parameter(ParameterSetName = "String")]
    [Parameter(Mandatory = true, ParameterSetName = "SignFolder")]
    [Parameter(Mandatory = true, ParameterSetName = "SignFile")]
    [Parameter(Mandatory = true, ParameterSetName = "SignString")]
    public FileInfo SignKey { get; set; }

    /// <summary>Password for the signing private key.</summary>
    [Parameter(ParameterSetName = "Folder")]
    [Parameter(ParameterSetName = "File")]
    [Parameter(ParameterSetName = "String")]
    [Parameter(ParameterSetName = "SignFolder")]
    [Parameter(ParameterSetName = "SignFile")]
    [Parameter(ParameterSetName = "SignString")]
    public string SignPassword { get; set; }

    /// <summary>Optional hash algorithm for encryption.</summary>
    [Parameter]
    [Alias("HashAlgorithmTag")]
    public HashAlgorithmTag? HashAlgorithm { get; set; }

    /// <summary>Optional compression algorithm for encryption.</summary>
    [Parameter]
    public CompressionAlgorithmTag? CompressionAlgorithm { get; set; }

    /// <summary>Type of data being encrypted.</summary>
    [Parameter]
    public PgpCore.Enums.PGPFileType? FileType { get; set; }

    /// <summary>PGP signature type when signing data.</summary>
    [Parameter]
    public int? PgpSignatureType { get; set; }

    /// <summary>Public key algorithm used during encryption.</summary>
    [Parameter]
    public PublicKeyAlgorithmTag? PublicKeyAlgorithm { get; set; }

    /// <summary>Symmetric key algorithm used during encryption.</summary>
    [Parameter]
    public SymmetricKeyAlgorithmTag? SymmetricKeyAlgorithm { get; set; }

    /// <summary>
    /// When specified, only a signature is produced instead of encrypting
    /// the input. This parameter is automatically implied when using the
    /// <c>Sign*</c> parameter sets.
    /// </summary>
    [Parameter(Mandatory = true, ParameterSetName = "SignFolder")]
    [Parameter(Mandatory = true, ParameterSetName = "SignFile")]
    [Parameter(Mandatory = true, ParameterSetName = "SignString")]
    public SwitchParameter SignOnly { get; set; }

    protected override void ProcessRecord() {
        try {
            bool signOnlyMode = SignOnly.IsPresent || ParameterSetName.StartsWith("Sign", System.StringComparison.OrdinalIgnoreCase);

            var publicKeys = new List<FileInfo>();
            if (!signOnlyMode) {
                foreach (var path in FilePathPublic) {
                    string resolved = PathResolver.Resolve(this, path);
                    if (File.Exists(resolved)) {
                        DateTime? expiration = KeyExpirationHelper.GetExpiration(resolved);
                        KeyExpirationHelper.WarnIfExpired(this, resolved, expiration);
                        publicKeys.Add(new FileInfo(resolved));
                    } else {
                        ErrorActionHelper.WriteErrorOrWarning(
                            this,
                            new FileNotFoundException($"Public key doesn't exist {resolved}"),
                            "PublicKeyNotFound",
                            ErrorCategory.InvalidArgument,
                            resolved,
                            $"Public key doesn't exist {resolved}");
                        return;
                    }
                }
            }

            if (SignKey != null && SignKey.Exists) {
                DateTime? expiration = KeyExpirationHelper.GetExpiration(SignKey.FullName);
                KeyExpirationHelper.WarnIfExpired(this, SignKey.FullName, expiration);
            }

            EncryptionKeys encryptionKeys = signOnlyMode
                ? new EncryptionKeys(SignKey, SignPassword)
                : SignKey != null
                    ? new EncryptionKeys(publicKeys, SignKey, SignPassword)
                    : new EncryptionKeys(publicKeys);
            var pgp = new PGP(encryptionKeys);

            PGPConfigurator.Configure(pgp, HashAlgorithm, CompressionAlgorithm, FileType, PgpSignatureType, PublicKeyAlgorithm, SymmetricKeyAlgorithm);

            if (ParameterSetName == "Folder" || ParameterSetName == "SignFolder") {
                string resolvedFolder = PathResolver.Resolve(this, FolderPath);
                foreach (var file in Directory.GetFiles(resolvedFolder, "*", SearchOption.AllDirectories)) {
                    string extension = signOnlyMode ? ".sig" : ".pgp";
                    string outputFile = !string.IsNullOrEmpty(OutputFolderPath)
                        ? Path.Combine(PathResolver.Resolve(this, OutputFolderPath), Path.GetFileName(file) + extension)
                        : file + extension;

                    if (signOnlyMode) {
                        pgp.SignFile(new FileInfo(file), new FileInfo(outputFile));
                    } else if (SignKey != null) {
                        pgp.EncryptFileAndSign(new FileInfo(file), new FileInfo(outputFile));
                    } else {
                        pgp.EncryptFile(new FileInfo(file), new FileInfo(outputFile));
                    }
                }
            } else if (ParameterSetName == "File" || ParameterSetName == "SignFile") {
                string resolvedFile = PathResolver.Resolve(this, FilePath);
                string extension = signOnlyMode ? ".sig" : ".pgp";
                string outputFile = !string.IsNullOrEmpty(OutFilePath) ? PathResolver.Resolve(this, OutFilePath) : resolvedFile + extension;

                if (signOnlyMode) {
                    pgp.SignFile(new FileInfo(resolvedFile), new FileInfo(outputFile));
                } else if (SignKey != null) {
                    pgp.EncryptFileAndSign(new FileInfo(resolvedFile), new FileInfo(outputFile));
                } else {
                    pgp.EncryptFile(new FileInfo(resolvedFile), new FileInfo(outputFile));
                }
            } else if (ParameterSetName == "String" || ParameterSetName == "SignString") {
                string result = signOnlyMode
                    ? pgp.SignArmoredString(String)
                    : SignKey != null
                        ? pgp.EncryptArmoredStringAndSign(String)
                        : pgp.EncryptArmoredString(String);
                WriteObject(result);
            }
        } catch (Exception ex) {
            WriteError(new ErrorRecord(ex, "ProtectPGPFailed", ErrorCategory.NotSpecified, null));
        }
    }
}