using Org.BouncyCastle.Bcpg;
using PgpCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace PSPGP;
/// <summary>
/// <para>Encrypts files, folders or strings using one or more public keys.</para>
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
/// </summary>
[Cmdlet("Protect", "PGP", DefaultParameterSetName = "File")]
public class CmdletProtectPGP : PSCmdlet {
    /// <summary>Public key files used for encryption.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Folder")]
    [Parameter(Mandatory = true, ParameterSetName = "File")]
    [Parameter(Mandatory = true, ParameterSetName = "String")]
    public string[] FilePathPublic { get; set; }

    /// <summary>Folder to encrypt when using the Folder parameter set.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Folder")]
    public string FolderPath { get; set; }

    /// <summary>Destination folder for encrypted files.</summary>
    [Parameter(ParameterSetName = "Folder")]
    public string OutputFolderPath { get; set; }

    /// <summary>File to encrypt when using the File parameter set.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "File")]
    public string FilePath { get; set; }

    /// <summary>Output file path for the encrypted file.</summary>
    [Parameter(ParameterSetName = "File")]
    public string OutFilePath { get; set; }

    /// <summary>String content to encrypt.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "String")]
    public string String { get; set; }

    /// <summary>Private key used for signing the encrypted data.</summary>
    [Parameter]
    public FileInfo SignKey { get; set; }

    /// <summary>Password for the signing private key.</summary>
    [Parameter]
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

    protected override void ProcessRecord() {
        try {
            var publicKeys = new List<FileInfo>();
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

            if (SignKey != null && SignKey.Exists) {
                DateTime? expiration = KeyExpirationHelper.GetExpiration(SignKey.FullName);
                KeyExpirationHelper.WarnIfExpired(this, SignKey.FullName, expiration);
            }

            EncryptionKeys encryptionKeys = SignKey != null ? new EncryptionKeys(publicKeys, SignKey, SignPassword) : new EncryptionKeys(publicKeys);
            var pgp = new PGP(encryptionKeys);

            PGPConfigurator.Configure(pgp, HashAlgorithm, CompressionAlgorithm, FileType, PgpSignatureType, PublicKeyAlgorithm, SymmetricKeyAlgorithm);

            if (ParameterSetName == "Folder") {
                string resolvedFolder = PathResolver.Resolve(this, FolderPath);
                foreach (var file in Directory.GetFiles(resolvedFolder, "*", SearchOption.AllDirectories)) {
                    string outputFile;
                    if (!string.IsNullOrEmpty(OutputFolderPath)) {
                        string resolvedOutput = PathResolver.Resolve(this, OutputFolderPath);
                        outputFile = Path.Combine(resolvedOutput, Path.GetFileName(file) + ".pgp");
                    } else {
                        outputFile = file + ".pgp";
                    }

                    if (SignKey != null) {
                        pgp.EncryptFileAndSign(new FileInfo(file), new FileInfo(outputFile));
                    } else {
                        pgp.EncryptFile(new FileInfo(file), new FileInfo(outputFile));
                    }
                }
            } else if (ParameterSetName == "File") {
                string resolvedFile = PathResolver.Resolve(this, FilePath);
                string outputFile = !string.IsNullOrEmpty(OutFilePath) ? PathResolver.Resolve(this, OutFilePath) : resolvedFile + ".pgp";

                if (SignKey != null) {
                    pgp.EncryptFileAndSign(new FileInfo(resolvedFile), new FileInfo(outputFile));
                } else {
                    pgp.EncryptFile(new FileInfo(resolvedFile), new FileInfo(outputFile));
                }
            } else if (ParameterSetName == "String") {
                string result = SignKey != null ? pgp.EncryptArmoredStringAndSign(String) : pgp.EncryptArmoredString(String);
                WriteObject(result);
            }
        } catch (Exception ex) {
            WriteError(new ErrorRecord(ex, "ProtectPGPFailed", ErrorCategory.NotSpecified, null));
        }
    }
}