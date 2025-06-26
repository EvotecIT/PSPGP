using PgpCore;
using System;
using System.IO;
using System.Management.Automation;

namespace PSPGP;
/// <summary>
/// <para>Removes PGP encryption from files or strings using a private key.</para>
/// <example>
/// <code>
/// Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP.asc -Password 'secret' -FolderPath $PSScriptRoot\Encoded -OutputFolderPath $PSScriptRoot\Decoded
/// </code>
/// </example>
/// <example>
/// <code>
/// Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP.asc -Password 'secret' -String $Encrypted
/// </code>
/// </example>
/// </summary>
[Cmdlet("Unprotect", "PGP", DefaultParameterSetName = "FolderClearText")]
public class CmdletUnprotectPGP : PSCmdlet {
    /// <summary>Private key file used to decrypt data.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FolderCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "FileCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringCredential")]
    public string FilePathPrivate { get; set; }

    /// <summary>Password protecting the private key.</summary>
    [Parameter(ParameterSetName = "FolderClearText")]
    [Parameter(ParameterSetName = "FileClearText")]
    [Parameter(ParameterSetName = "StringClearText")]
    public string Password { get; set; }

    /// <summary>Credential object with password for the private key.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FileCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "StringCredential")]
    public PSCredential Credential { get; set; }

    /// <summary>Folder containing encrypted files.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FolderCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderClearText")]
    public string FolderPath { get; set; }

    /// <summary>Destination folder for decrypted output.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FolderCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FolderClearText")]
    public string OutputFolderPath { get; set; }

    /// <summary>Encrypted file to decrypt.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FileCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileClearText")]
    public string FilePath { get; set; }

    /// <summary>Output file path for decrypted data.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "FileCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "FileClearText")]
    public string OutFilePath { get; set; }

    /// <summary>Encrypted text to decrypt.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "StringClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "StringCredential")]
    public string String { get; set; }

    protected override void ProcessRecord() {
        try {
            string resolvedPrivate = PathResolver.Resolve(this, FilePathPrivate);
            if (!File.Exists(resolvedPrivate)) {
                WriteWarning("Unprotect-PGP - Remove PGP encryption failed because private key file doesn't exists.");
                return;
            }
            string privateKey = File.ReadAllText(resolvedPrivate);
            string password = Password;
            if (Credential != null) {
                password = Credential.GetNetworkCredential().Password;
            }

            var encryptionKeys = new EncryptionKeys(privateKey, password);
            var pgp = new PGP(encryptionKeys);

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
                        pgp.DecryptFile(new FileInfo(file), new FileInfo(outputFile));
                    } catch (Exception ex) {
                        WriteError(new ErrorRecord(ex, "DecryptFileFailed", ErrorCategory.NotSpecified, file));
                        return;
                    }
                }
            } else if (ParameterSetName.StartsWith("File")) {
                try {
                    string resolvedFile = PathResolver.Resolve(this, FilePath);
                    string outputFile = !string.IsNullOrEmpty(OutFilePath) ? PathResolver.Resolve(this, OutFilePath) : resolvedFile.Replace(".pgp", string.Empty);
                    pgp.DecryptFile(new FileInfo(resolvedFile), new FileInfo(outputFile));
                } catch (Exception ex) {
                    WriteError(new ErrorRecord(ex, "DecryptFileFailed", ErrorCategory.NotSpecified, FilePath));
                    return;
                }
            } else if (ParameterSetName.StartsWith("String")) {
                try {
                    string result = pgp.DecryptArmoredString(String);
                    WriteObject(result);
                } catch (Exception ex) {
                    WriteError(new ErrorRecord(ex, "DecryptStringFailed", ErrorCategory.NotSpecified, null));
                }
            }
        } catch (Exception ex) {
            WriteError(new ErrorRecord(ex, "UnprotectPGPFailed", ErrorCategory.NotSpecified, null));
        }
    }
}