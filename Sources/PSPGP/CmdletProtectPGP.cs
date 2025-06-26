using Org.BouncyCastle.Bcpg;
using PgpCore;
using PSPGP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace PSPGP.Cmdlets; 
[Cmdlet("Protect", "PGP", DefaultParameterSetName = "File")]
public class CmdletProtectPGP : PSCmdlet {
    [Parameter(Mandatory = true, ParameterSetName = "Folder")]
    [Parameter(Mandatory = true, ParameterSetName = "File")]
    [Parameter(Mandatory = true, ParameterSetName = "String")]
    public string[] FilePathPublic { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = "Folder")]
    public string FolderPath { get; set; }

    [Parameter(ParameterSetName = "Folder")]
    public string OutputFolderPath { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = "File")]
    public string FilePath { get; set; }

    [Parameter(ParameterSetName = "File")]
    public string OutFilePath { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = "String")]
    public string String { get; set; }

    [Parameter]
    public FileInfo SignKey { get; set; }

    [Parameter]
    public string SignPassword { get; set; }

    [Parameter]
    [Alias("HashAlgorithmTag")]
    public HashAlgorithmTag? HashAlgorithm { get; set; }

    [Parameter]
    public CompressionAlgorithmTag? CompressionAlgorithm { get; set; }

    [Parameter]
    public PgpCore.Enums.PGPFileType? FileType { get; set; }

    [Parameter]
    public int? PgpSignatureType { get; set; }

    [Parameter]
    public PublicKeyAlgorithmTag? PublicKeyAlgorithm { get; set; }

    [Parameter]
    public SymmetricKeyAlgorithmTag? SymmetricKeyAlgorithm { get; set; }

    protected override void ProcessRecord() {
        try {
            var publicKeys = new List<FileInfo>();
            foreach (var path in FilePathPublic) {
                string resolved = PathResolver.Resolve(this, path);
                if (File.Exists(resolved)) {
                    publicKeys.Add(new FileInfo(resolved));
                } else {
                    WriteError(new ErrorRecord(new FileNotFoundException($"Public key doesn't exist {resolved}"), "PublicKeyNotFound", ErrorCategory.InvalidArgument, resolved));
                    return;
                }
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