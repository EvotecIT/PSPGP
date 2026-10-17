using PgpCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;

namespace PSPGP;
/// <summary>
/// <para>Removes PGP encryption from files or strings using a private key or symmetric passphrase.</para>
/// </summary>
/// <example>
/// <code>
/// Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -Password 'secret' -FolderPath $PSScriptRoot\Encoded -OutputFolderPath $PSScriptRoot\Decoded
/// </code>
/// </example>
/// <example>
/// <code>
/// Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -Password 'secret' -String $Encrypted
/// </code>
/// </example>
/// <example>
/// <code>
/// Unprotect-PGP -SymmetricPassphrase 'SymmetricPass123!' -String $Encrypted
/// </code>
/// </example>
[Cmdlet("Unprotect", "PGP", DefaultParameterSetName = "FolderClearText", SupportsShouldProcess = true)]
public class CmdletUnprotectPGP : PSCmdlet {
    /// <summary>Private key file used to decrypt data.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FolderCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "FileCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyCredential")]
    public string[] FilePathPrivate { get; set; }

    /// <summary>Trusted public key files used to verify encrypted and signed content.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyCredential")]
    public string[] FilePathPublic { get; set; }

    /// <summary>Passphrase used for symmetric decryption.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FolderSymmetric")]
    [Parameter(Mandatory = true, ParameterSetName = "FileSymmetric")]
    [Parameter(Mandatory = true, ParameterSetName = "StringSymmetric")]
    public string SymmetricPassphrase { get; set; }

    /// <summary>Password protecting the private key.</summary>
    [Parameter(ParameterSetName = "FolderClearText")]
    [Parameter(ParameterSetName = "FileClearText")]
    [Parameter(ParameterSetName = "StringClearText")]
    [Parameter(ParameterSetName = "FolderVerifyClearText")]
    [Parameter(ParameterSetName = "FileVerifyClearText")]
    [Parameter(ParameterSetName = "StringVerifyClearText")]
    public string Password { get; set; }

    /// <summary>Credential object with password for the private key.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FileCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "StringCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyCredential")]
    public PSCredential Credential { get; set; }

    /// <summary>Folder containing encrypted files.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FolderCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderSymmetric")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyClearText")]
    public string FolderPath { get; set; }

    /// <summary>Destination folder for decrypted output.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FolderCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderSymmetric")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyClearText")]
    public string OutputFolderPath { get; set; }

    /// <summary>Encrypted file to decrypt.</summary>
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "FileCredential")]
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "FileClearText")]
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "FileSymmetric")]
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "FileVerifyCredential")]
    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "FileVerifyClearText")]
    [Alias("FullName", "LiteralPath")]
    public string FilePath { get; set; }

    /// <summary>Output file path for decrypted data.</summary>
    [Parameter(ParameterSetName = "FileCredential")]
    [Parameter(ParameterSetName = "FileClearText")]
    [Parameter(ParameterSetName = "FileSymmetric")]
    [Parameter(ParameterSetName = "FileVerifyCredential")]
    [Parameter(ParameterSetName = "FileVerifyClearText")]
    public string OutFilePath { get; set; }

    /// <summary>Encrypted text to decrypt.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "StringClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "StringSymmetric")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyCredential")]
    public string String { get; set; }

    /// <summary>Verifies the signature inside encrypted content before returning plaintext.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyCredential")]
    public SwitchParameter Verify { get; set; }

    /// <summary>Ignores modification-detection/integrity-check failures during decryption.</summary>
    [Parameter]
    public SwitchParameter IgnoreIntegrityCheckFailure { get; set; }

    /// <summary>Returns the completed output file for each successful file operation.</summary>
    [Parameter(ParameterSetName = "FolderCredential")]
    [Parameter(ParameterSetName = "FolderClearText")]
    [Parameter(ParameterSetName = "FolderSymmetric")]
    [Parameter(ParameterSetName = "FolderVerifyCredential")]
    [Parameter(ParameterSetName = "FolderVerifyClearText")]
    [Parameter(ParameterSetName = "FileCredential")]
    [Parameter(ParameterSetName = "FileClearText")]
    [Parameter(ParameterSetName = "FileSymmetric")]
    [Parameter(ParameterSetName = "FileVerifyCredential")]
    [Parameter(ParameterSetName = "FileVerifyClearText")]
    public SwitchParameter PassThru { get; set; }

    /// <summary>
    /// Decrypts files or strings using the supplied private keys
    /// and writes the decrypted data to disk or the pipeline.
    /// </summary>
    protected override void ProcessRecord() {
        bool verifyMode = Verify.IsPresent;

        try {
            var resolvedPrivates = new List<string>();
            var resolvedPublics = new List<string>();
            bool symmetricMode = ParameterSetName.EndsWith("Symmetric", System.StringComparison.OrdinalIgnoreCase);
            if (!symmetricMode) {
                foreach (var path in FilePathPrivate) {
                    string resolved = PathResolver.Resolve(this, path);
                    if (!File.Exists(resolved)) {
                        CmdletError.Write(
                            this,
                            new FileNotFoundException($"Private key doesn't exist {resolved}"),
                            "PrivateKeyNotFound",
                            ErrorCategory.InvalidArgument,
                            resolved);
                        return;
                    }
                    DateTime? expiration = KeyExpirationHelper.GetExpiration(resolved);
                    KeyExpirationHelper.WarnIfExpired(this, resolved, expiration);
                    resolvedPrivates.Add(resolved);
                }
            }

            if (verifyMode) {
                foreach (var path in FilePathPublic) {
                    string resolved = PathResolver.Resolve(this, path);
                    if (!File.Exists(resolved)) {
                        CmdletError.Write(
                            this,
                            new FileNotFoundException($"Public key doesn't exist {resolved}"),
                            "PublicKeyNotFound",
                            ErrorCategory.InvalidArgument,
                            resolved);
                        return;
                    }
                    DateTime? expiration = KeyExpirationHelper.GetExpiration(resolved);
                    KeyExpirationHelper.WarnIfExpired(this, resolved, expiration);
                    resolvedPublics.Add(resolved);
                }
            }

            string password = Password ?? string.Empty;
            if (Credential != null) {
                password = Credential.GetNetworkCredential().Password;
            }

            if (ParameterSetName.StartsWith("Folder", StringComparison.Ordinal)) {
                var plan = FileWorkflow.Plan(PathResolver.Resolve(this, FolderPath), PathResolver.Resolve(this, OutputFolderPath), FileWorkflow.RemoveEncryptedSuffix);
                foreach (var item in plan) {
                    ProcessFile(item.Input, item.Output, resolvedPrivates, password, resolvedPublics, symmetricMode, verifyMode);
                }
            } else if (ParameterSetName.StartsWith("File", StringComparison.Ordinal)) {
                string file = PathResolver.Resolve(this, FilePath);
                string output = string.IsNullOrEmpty(OutFilePath) ? FileWorkflow.RemoveEncryptedSuffix(file) : PathResolver.Resolve(this, OutFilePath);
                ProcessFile(file, output, resolvedPrivates, password, resolvedPublics, symmetricMode, verifyMode);
            } else if (ParameterSetName.StartsWith("String")) {
                try {
                    bool decrypted = false;
                    string result = null;
                    Exception lastError = null;
                    if (symmetricMode) {
                        try {
                            var encryptionKeys = new EncryptionKeys(Encoding.UTF8.GetBytes(SymmetricPassphrase));
                            var pgp = new PGP(encryptionKeys);
                            ConfigureDecryption(pgp);
                            result = pgp.DecryptArmoredString(String);
                            decrypted = true;
                        } catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
                            lastError = PgpExceptionHelper.Normalize(ex);
                        }
                    } else {
                        foreach (var key in resolvedPrivates) {
                            try {
                                using var privateKeyStream = KeyMaterialHelper.OpenRead(key);
                                List<Stream> publicKeyStreams = null;
                                try {
                                    var encryptionKeys = CreateEncryptionKeys(privateKeyStream, password, resolvedPublics, verifyMode, out publicKeyStreams);
                                    var pgp = new PGP(encryptionKeys);
                                    ConfigureDecryption(pgp);
                                    result = verifyMode
                                        ? pgp.DecryptArmoredStringAndVerify(String)
                                        : pgp.DecryptArmoredString(String);
                                } finally {
                                    DisposeStreams(publicKeyStreams);
                                }
                                decrypted = true;
                                break;
                            } catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
                                lastError = PgpExceptionHelper.Normalize(ex, key);
                            }
                        }
                    }

                    if (decrypted) {
                        WriteObject(result);
                    } else {
                        WriteError(PgpExceptionHelper.CreateErrorRecord(lastError, "DecryptStringFailed"));
                    }
                } catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
                    WriteError(PgpExceptionHelper.CreateErrorRecord(ex, "DecryptStringFailed"));
                }
            }
        } catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
            WriteError(PgpExceptionHelper.CreateErrorRecord(ex, "UnprotectPGPFailed"));
        }
    }

    private void ConfigureDecryption(PGP pgp) {
        if (IgnoreIntegrityCheckFailure.IsPresent) {
            pgp.IgnoreIntegrityCheckFailure = true;
        }
    }

    private static EncryptionKeys CreateEncryptionKeys(Stream privateKeyStream, string password, List<string> publicKeyPaths, bool verifyMode, out List<Stream> publicKeyStreams) {
        publicKeyStreams = null;
        if (!verifyMode) {
            return new EncryptionKeys(privateKeyStream, password);
        }

        publicKeyStreams = OpenStreams(publicKeyPaths);
        return new EncryptionKeys(publicKeyStreams, privateKeyStream, password);
    }

    private static List<Stream> OpenStreams(List<string> paths) {
        var streams = new List<Stream>();
        try {
            foreach (string path in paths) {
                streams.Add(KeyMaterialHelper.OpenRead(path));
            }
        } catch {
            DisposeStreams(streams);
            throw;
        }

        return streams;
    }

    private static void DisposeStreams(List<Stream> streams) {
        if (streams == null) {
            return;
        }

        foreach (Stream stream in streams) {
            stream.Dispose();
        }
    }

    private void ProcessFile(string input, string output, List<string> privateKeys, string password, List<string> publicKeys, bool symmetric, bool verify) {
        if (!ShouldProcess(output, verify ? "Write decrypted and verified file" : "Write decrypted file")) return;
        FileWorkflow.EnsureOutputDirectory(input, output);
        Exception lastError = null;
        IEnumerable<string> candidates = symmetric ? new string[] { null } : privateKeys;
        foreach (string key in candidates) {
            List<Stream> publicStreams = null;
            try {
                using var privateStream = symmetric ? null : KeyMaterialHelper.OpenRead(key);
                var keys = symmetric ? new EncryptionKeys(Encoding.UTF8.GetBytes(SymmetricPassphrase))
                    : CreateEncryptionKeys(privateStream, password, publicKeys, verify, out publicStreams);
                var pgp = new PGP(keys);
                ConfigureDecryption(pgp);
                if (verify) pgp.DecryptFileAndVerify(new FileInfo(input), new FileInfo(output));
                else pgp.DecryptFile(new FileInfo(input), new FileInfo(output));
                if (PassThru.IsPresent) WriteObject(new FileInfo(output));
                return;
            } catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
                lastError = PgpExceptionHelper.Normalize(ex, key);
            } finally {
                DisposeStreams(publicStreams);
            }
        }
        WriteError(PgpExceptionHelper.CreateErrorRecord(lastError, "DecryptFileFailed", input));
    }
}
