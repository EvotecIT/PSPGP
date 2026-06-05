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
[Cmdlet("Unprotect", "PGP", DefaultParameterSetName = "FolderClearText")]
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

    /// <summary>Public key files reserved for signed-and-encrypted verification workflows.</summary>
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
    [Parameter(Mandatory = true, ParameterSetName = "FileCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "FileSymmetric")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyClearText")]
    public string FilePath { get; set; }

    /// <summary>Output file path for decrypted data.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FileCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "FileSymmetric")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyClearText")]
    public string OutFilePath { get; set; }

    /// <summary>Encrypted text to decrypt.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "StringClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "StringSymmetric")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyCredential")]
    public string String { get; set; }

    /// <summary>Reserved for future signed-and-encrypted verification support.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringVerifyCredential")]
    public SwitchParameter Verify { get; set; }

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
                        ErrorActionHelper.WriteErrorOrWarning(
                            this,
                            new FileNotFoundException($"Private key doesn't exist {resolved}"),
                            "PrivateKeyNotFound",
                            ErrorCategory.InvalidArgument,
                            resolved,
                            $"Private key doesn't exist {resolved}");
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
                    resolvedPublics.Add(resolved);
                }
            }

            string password = Password;
            if (Credential != null) {
                password = Credential.GetNetworkCredential().Password;
            }

            if (ParameterSetName.StartsWith("Folder")) {
                string resolvedFolder = PathResolver.Resolve(this, FolderPath);
                foreach (var file in Directory.GetFiles(resolvedFolder, "*", SearchOption.AllDirectories)) {
                    try {
                        string outputFile;
                        if (!string.IsNullOrEmpty(OutputFolderPath)) {
                            string resolvedOutput = PathResolver.Resolve(this, OutputFolderPath);
                            outputFile = Path.Combine(resolvedOutput, Path.GetFileName(file).Replace(".pgp", string.Empty));
                        } else {
                            outputFile = file.Replace(".pgp", string.Empty);
                        }

                        bool decrypted = false;
                        Exception lastError = null;
                        if (symmetricMode) {
                            try {
                                var encryptionKeys = new EncryptionKeys(Encoding.UTF8.GetBytes(SymmetricPassphrase));
                                var pgp = new PGP(encryptionKeys);
                                pgp.DecryptFile(new FileInfo(file), new FileInfo(outputFile));
                                decrypted = true;
                            } catch (Exception ex) {
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
                                        if (verifyMode) {
                                            DecryptFileAndVerifyToOutput(pgp, file, outputFile);
                                        } else {
                                            pgp.DecryptFile(new FileInfo(file), new FileInfo(outputFile));
                                        }
                                    } finally {
                                        DisposeStreams(publicKeyStreams);
                                    }
                                    decrypted = true;
                                    break;
                                } catch (Exception ex) {
                                    lastError = PgpExceptionHelper.Normalize(ex, key);
                                }
                            }
                        }

                        if (!decrypted) {
                            WriteError(new ErrorRecord(lastError, "DecryptFileFailed", ErrorCategory.NotSpecified, file));
                        }
                    } catch (Exception ex) {
                        WriteError(new ErrorRecord(ex, "DecryptFileFailed", ErrorCategory.NotSpecified, file));
                        return;
                    }
                }
            } else if (ParameterSetName.StartsWith("File")) {
                try {
                    string resolvedFile = PathResolver.Resolve(this, FilePath);
                    string outputFile = !string.IsNullOrEmpty(OutFilePath) ? PathResolver.Resolve(this, OutFilePath) : resolvedFile.Replace(".pgp", string.Empty);

                    bool decrypted = false;
                    Exception lastError = null;
                    if (symmetricMode) {
                        try {
                            var encryptionKeys = new EncryptionKeys(Encoding.UTF8.GetBytes(SymmetricPassphrase));
                            var pgp = new PGP(encryptionKeys);
                            pgp.DecryptFile(new FileInfo(resolvedFile), new FileInfo(outputFile));
                            decrypted = true;
                        } catch (Exception ex) {
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
                                    if (verifyMode) {
                                        DecryptFileAndVerifyToOutput(pgp, resolvedFile, outputFile);
                                    } else {
                                        pgp.DecryptFile(new FileInfo(resolvedFile), new FileInfo(outputFile));
                                    }
                                } finally {
                                    DisposeStreams(publicKeyStreams);
                                }
                                decrypted = true;
                                break;
                            } catch (Exception ex) {
                                lastError = PgpExceptionHelper.Normalize(ex, key);
                            }
                        }
                    }

                    if (!decrypted) {
                        WriteError(new ErrorRecord(lastError, "DecryptFileFailed", ErrorCategory.NotSpecified, FilePath));
                    }
                } catch (Exception ex) {
                    WriteError(new ErrorRecord(ex, "DecryptFileFailed", ErrorCategory.NotSpecified, FilePath));
                    return;
                }
            } else if (ParameterSetName.StartsWith("String")) {
                try {
                    bool decrypted = false;
                    string result = null;
                    Exception lastError = null;
                    if (symmetricMode) {
                        try {
                            var encryptionKeys = new EncryptionKeys(Encoding.UTF8.GetBytes(SymmetricPassphrase));
                            var pgp = new PGP(encryptionKeys);
                            result = pgp.DecryptArmoredString(String);
                            decrypted = true;
                        } catch (Exception ex) {
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
                                    result = verifyMode
                                        ? pgp.DecryptArmoredStringAndVerify(String)
                                        : pgp.DecryptArmoredString(String);
                                } finally {
                                    DisposeStreams(publicKeyStreams);
                                }
                                decrypted = true;
                                break;
                            } catch (Exception ex) {
                                lastError = PgpExceptionHelper.Normalize(ex, key);
                            }
                        }
                    }

                    if (decrypted) {
                        WriteObject(result);
                    } else {
                        WriteError(new ErrorRecord(lastError, "DecryptStringFailed", ErrorCategory.NotSpecified, null));
                    }
                } catch (Exception ex) {
                    WriteError(new ErrorRecord(ex, "DecryptStringFailed", ErrorCategory.NotSpecified, null));
                }
            }
        } catch (Exception ex) {
            WriteError(new ErrorRecord(ex, "UnprotectPGPFailed", ErrorCategory.NotSpecified, null));
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

    private static void DecryptFileAndVerifyToOutput(PGP pgp, string inputFile, string outputFile) {
        string tempOutputFile = GetTemporaryOutputFile(outputFile);
        try {
            pgp.DecryptFileAndVerify(new FileInfo(inputFile), new FileInfo(tempOutputFile));
            if (File.Exists(outputFile)) {
                File.Delete(outputFile);
            }
            File.Move(tempOutputFile, outputFile);
        } finally {
            if (File.Exists(tempOutputFile)) {
                File.Delete(tempOutputFile);
            }
        }
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
