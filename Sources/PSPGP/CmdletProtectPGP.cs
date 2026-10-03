using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PgpCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;

namespace PSPGP;
/// <summary>
/// <para>
/// Encrypts or signs files, folders or strings using one or more public keys.
/// Use <c>-SignOnly</c> or the <c>Sign*</c> parameter sets to create signatures
/// without encryption. Add <c>-Detached</c> to create a separate detached signature.
/// </para>
/// </summary>
/// <example>
/// <code>
/// Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FolderPath $PSScriptRoot\Test -OutputFolderPath $PSScriptRoot\Encoded
/// </code>
/// </example>
/// <example>
/// <code>
/// Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePath $PSScriptRoot\Test\Test1.txt -OutFilePath $PSScriptRoot\Encoded\Test1.txt.pgp
/// </code>
/// </example>
/// <example>
/// <code>
/// Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String "Sensitive text"
/// </code>
/// </example>
/// <example>
/// <code>
/// Protect-PGP -SymmetricPassphrase 'SymmetricPass123!' -String 'Sensitive text'
/// </code>
/// </example>
/// <example>
/// <code>
/// Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'secret' -String "Signed content"
/// </code>
/// </example>
/// <example>
/// <code>
/// Protect-PGP -ClearSign -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'secret' -String "Human readable signed content"
/// </code>
/// </example>
[Cmdlet("Protect", "PGP", DefaultParameterSetName = "File", SupportsShouldProcess = true)]
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
    [Parameter(Mandatory = true, ParameterSetName = "ClearSignFolder")]
    [Parameter(Mandatory = true, ParameterSetName = "SymmetricFolder")]
    public string FolderPath { get; set; }

    /// <summary>Destination folder for encrypted files.</summary>
    [Parameter(ParameterSetName = "Folder")]
    [Parameter(ParameterSetName = "SignFolder")]
    [Parameter(ParameterSetName = "ClearSignFolder")]
    [Parameter(ParameterSetName = "SymmetricFolder")]
    public string OutputFolderPath { get; set; }

    /// <summary>File to encrypt when using the File parameter set.</summary>
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "File")]
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "SignFile")]
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "ClearSignFile")]
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "SymmetricFile")]
    [Alias("FullName", "LiteralPath")]
    public string FilePath { get; set; }

    /// <summary>Output file path for the encrypted file.</summary>
    [Parameter(ParameterSetName = "File")]
    [Parameter(ParameterSetName = "SignFile")]
    [Parameter(ParameterSetName = "ClearSignFile")]
    [Parameter(ParameterSetName = "SymmetricFile")]
    public string OutFilePath { get; set; }

    /// <summary>String content to encrypt.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "String")]
    [Parameter(Mandatory = true, ParameterSetName = "SignString")]
    [Parameter(Mandatory = true, ParameterSetName = "ClearSignString")]
    [Parameter(Mandatory = true, ParameterSetName = "SymmetricString")]
    public string String { get; set; }

    /// <summary>Passphrase used for symmetric encryption.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "SymmetricFolder")]
    [Parameter(Mandatory = true, ParameterSetName = "SymmetricFile")]
    [Parameter(Mandatory = true, ParameterSetName = "SymmetricString")]
    public string SymmetricPassphrase { get; set; }

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
    [Parameter(Mandatory = true, ParameterSetName = "ClearSignFolder")]
    [Parameter(Mandatory = true, ParameterSetName = "ClearSignFile")]
    [Parameter(Mandatory = true, ParameterSetName = "ClearSignString")]
    public FileInfo SignKey { get; set; }

    /// <summary>Password for the signing private key.</summary>
    [Parameter(ParameterSetName = "Folder")]
    [Parameter(ParameterSetName = "File")]
    [Parameter(ParameterSetName = "String")]
    [Parameter(ParameterSetName = "SignFolder")]
    [Parameter(ParameterSetName = "SignFile")]
    [Parameter(ParameterSetName = "SignString")]
    [Parameter(ParameterSetName = "ClearSignFolder")]
    [Parameter(ParameterSetName = "ClearSignFile")]
    [Parameter(ParameterSetName = "ClearSignString")]
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

    /// <summary>Compatibility parameter; supplying it is rejected. Existing keys determine their own algorithm.</summary>
    [Parameter]
    public PublicKeyAlgorithmTag? PublicKeyAlgorithm { get; set; }

    /// <summary>Symmetric key algorithm used during encryption.</summary>
    [Parameter]
    public SymmetricKeyAlgorithmTag? SymmetricKeyAlgorithm { get; set; }

    /// <summary>Controls whether file output is armored.</summary>
    [Parameter]
    public bool Armor { get; set; } = true;

    /// <summary>Controls whether an integrity check packet is added.</summary>
    [Parameter]
    public bool WithIntegrityCheck { get; set; } = true;

    /// <summary>Optional literal file name embedded in the PGP payload.</summary>
    [Parameter]
    [Alias("Name")]
    public string LiteralFileName { get; set; }

    /// <summary>Optional armored headers added to generated content.</summary>
    [Parameter]
    public Hashtable Headers { get; set; }

    /// <summary>Uses the legacy packet format when set.</summary>
    [Parameter]
    public SwitchParameter OldFormat { get; set; }

    /// <summary>Adds the PGP version header to generated armored content.</summary>
    [Parameter]
    public SwitchParameter AddVersionHeader { get; set; }

    /// <summary>
    /// When specified, only a signature is produced instead of encrypting
    /// the input. This parameter is automatically implied when using the
    /// <c>Sign*</c> parameter sets.
    /// </summary>
    [Parameter(Mandatory = true, ParameterSetName = "SignFolder")]
    [Parameter(Mandatory = true, ParameterSetName = "SignFile")]
    [Parameter(Mandatory = true, ParameterSetName = "SignString")]
    public SwitchParameter SignOnly { get; set; }

    /// <summary>Creates a detached signature over the original input.</summary>
    [Parameter(ParameterSetName = "SignFolder")]
    [Parameter(ParameterSetName = "SignFile")]
    [Parameter(ParameterSetName = "SignString")]
    public SwitchParameter Detached { get; set; }

    /// <summary>
    /// Creates a clear-signed message that remains human readable.
    /// </summary>
    [Parameter(Mandatory = true, ParameterSetName = "ClearSignFolder")]
    [Parameter(Mandatory = true, ParameterSetName = "ClearSignFile")]
    [Parameter(Mandatory = true, ParameterSetName = "ClearSignString")]
    public SwitchParameter ClearSign { get; set; }

    /// <summary>Returns the completed output file for each successful file operation.</summary>
    [Parameter(ParameterSetName = "Folder")]
    [Parameter(ParameterSetName = "File")]
    [Parameter(ParameterSetName = "SignFolder")]
    [Parameter(ParameterSetName = "SignFile")]
    [Parameter(ParameterSetName = "ClearSignFolder")]
    [Parameter(ParameterSetName = "ClearSignFile")]
    [Parameter(ParameterSetName = "SymmetricFolder")]
    [Parameter(ParameterSetName = "SymmetricFile")]
    public SwitchParameter PassThru { get; set; }

    /// <summary>
    /// Encrypts or signs input data based on the selected
    /// parameter set and writes results to files or the pipeline.
    /// </summary>
    protected override void ProcessRecord() {
        List<Stream> publicKeyStreams = new();
        Stream signKeyStream = null;
        try {
            bool signOnlyMode = SignOnly.IsPresent || ParameterSetName.StartsWith("Sign", System.StringComparison.OrdinalIgnoreCase);
            bool clearSignMode = ClearSign.IsPresent || ParameterSetName.StartsWith("ClearSign", System.StringComparison.OrdinalIgnoreCase);
            bool symmetricMode = ParameterSetName.StartsWith("Symmetric", System.StringComparison.OrdinalIgnoreCase);
            Dictionary<string, string> headers = HeaderHelper.ToDictionary(Headers);
            if (PublicKeyAlgorithm.HasValue)
                throw new ArgumentException("PublicKeyAlgorithm cannot change an existing key. Select it when generating the key with New-PGPKey.");
            if (PgpSignatureType.HasValue && PgpSignatureType.Value != (clearSignMode ? PgpSignature.CanonicalTextDocument : PgpSignature.BinaryDocument))
                throw new ArgumentException("Use ClearSign for canonical text signatures. Other signing operations produce binary-document signatures.");
            if (ParameterSetName.EndsWith("String", StringComparison.Ordinal) && !Armor)
                throw new ArgumentException("String operations produce armored text. Use file input for binary output.");

            var publicKeys = new List<string>();
            if (!signOnlyMode && !clearSignMode && !symmetricMode) {
                foreach (var path in FilePathPublic) {
                    string resolved = PathResolver.Resolve(this, path);
                    if (File.Exists(resolved)) {
                        DateTime? expiration = KeyExpirationHelper.GetExpiration(resolved);
                        KeyExpirationHelper.WarnIfExpired(this, resolved, expiration);
                        publicKeys.Add(resolved);
                    } else {
                        CmdletError.Write(
                            this,
                            new FileNotFoundException($"Public key doesn't exist {resolved}"),
                            "PublicKeyNotFound",
                            ErrorCategory.InvalidArgument,
                            resolved);
                        return;
                    }
                }
            }

            if (SignKey != null && File.Exists(PathResolver.Resolve(this, SignKey.ToString()))) {
                DateTime? expiration = KeyExpirationHelper.GetExpiration(PathResolver.Resolve(this, SignKey.ToString()));
                KeyExpirationHelper.WarnIfExpired(this, PathResolver.Resolve(this, SignKey.ToString()), expiration);
            }

            foreach (string publicKeyPath in publicKeys) {
                publicKeyStreams.Add(KeyMaterialHelper.OpenRead(publicKeyPath));
            }

            if (SignKey != null) {
                signKeyStream = KeyMaterialHelper.OpenRead(PathResolver.Resolve(this, SignKey.ToString()));
            }

            EncryptionKeys encryptionKeys = symmetricMode
                ? new EncryptionKeys(Encoding.UTF8.GetBytes(SymmetricPassphrase))
                : signOnlyMode || clearSignMode
                ? new EncryptionKeys(signKeyStream, SignPassword ?? string.Empty)
                : SignKey != null
                    ? new EncryptionKeys(publicKeyStreams, signKeyStream, SignPassword ?? string.Empty)
                    : new EncryptionKeys(publicKeyStreams);
            var pgp = new PGP(encryptionKeys);

            PGPConfigurator.Configure(pgp, HashAlgorithm, CompressionAlgorithm, FileType, PgpSignatureType, PublicKeyAlgorithm, SymmetricKeyAlgorithm);
            pgp.AddVersionHeader = AddVersionHeader.IsPresent;

            if (ParameterSetName.EndsWith("Folder", StringComparison.Ordinal)) {
                string extension = clearSignMode ? ".asc" : signOnlyMode ? ".sig" : ".pgp";
                var plan = FileWorkflow.Plan(PathResolver.Resolve(this, FolderPath),
                    string.IsNullOrEmpty(OutputFolderPath) ? null : PathResolver.Resolve(this, OutputFolderPath), name => name + extension);
                foreach (var item in plan) {
                    ProcessFile(pgp, item.Input, item.Output, clearSignMode, signOnlyMode, headers);
                }
            } else if (ParameterSetName.EndsWith("File", StringComparison.Ordinal)) {
                string file = PathResolver.Resolve(this, FilePath);
                string extension = clearSignMode ? ".asc" : signOnlyMode ? ".sig" : ".pgp";
                string output = string.IsNullOrEmpty(OutFilePath) ? file + extension : PathResolver.Resolve(this, OutFilePath);
                ProcessFile(pgp, file, output, clearSignMode, signOnlyMode, headers);
            } else if (ParameterSetName == "String" || ParameterSetName == "SignString" || ParameterSetName == "ClearSignString" || ParameterSetName == "SymmetricString") {
                string result = clearSignMode
                    ? pgp.ClearSignArmoredString(String, headers)
                    : signOnlyMode
                    ? Detached.IsPresent
                        ? pgp.SignDetached(String, headers)
                        : pgp.SignArmoredString(String, LiteralFileName, headers, OldFormat.IsPresent)
                    : SignKey != null
                        ? pgp.EncryptArmoredStringAndSign(String, WithIntegrityCheck, LiteralFileName, headers, OldFormat.IsPresent)
                        : pgp.EncryptArmoredString(String, WithIntegrityCheck, LiteralFileName, headers, OldFormat.IsPresent);
                WriteObject(result);
            }
        } catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
            string keyPath = null;
            if (SignKey is null && FilePathPublic != null && FilePathPublic.Length == 1) {
                keyPath = FilePathPublic[0];
            } else if (SignKey != null && (FilePathPublic is null || FilePathPublic.Length == 0)) {
                keyPath = PathResolver.Resolve(this, SignKey.ToString());
            }

            WriteError(PgpExceptionHelper.CreateErrorRecord(ex, "ProtectPGPFailed", keyPath, keyPath));
        } finally {
            signKeyStream?.Dispose();
            foreach (Stream stream in publicKeyStreams) {
                stream.Dispose();
            }
        }
    }
    private void ProcessFile(PGP pgp, string input, string output, bool clearSign, bool signOnly, IDictionary<string, string> headers) {
        if (!ShouldProcess(output, clearSign || signOnly ? "Write PGP signature" : "Write encrypted file")) return;
        FileWorkflow.EnsureOutputDirectory(input, output);
        if (clearSign) {
            pgp.ClearSignFile(new FileInfo(input), new FileInfo(output), headers);
        } else if (signOnly) {
            if (Detached.IsPresent) pgp.SignDetached(new FileInfo(input), new FileInfo(output), Armor, headers);
            else pgp.SignFile(new FileInfo(input), new FileInfo(output), Armor, LiteralFileName, headers, OldFormat.IsPresent);
        } else if (SignKey != null) {
            pgp.EncryptFileAndSign(new FileInfo(input), new FileInfo(output), Armor, WithIntegrityCheck, LiteralFileName, headers, OldFormat.IsPresent);
        } else {
            pgp.EncryptFile(new FileInfo(input), new FileInfo(output), Armor, WithIntegrityCheck, LiteralFileName, headers, OldFormat.IsPresent);
        }
        if (PassThru.IsPresent) WriteObject(new FileInfo(output));
    }

}
