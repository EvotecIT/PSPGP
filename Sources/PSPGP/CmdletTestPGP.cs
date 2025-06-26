using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using PgpCore;
using PSPGP.Helpers;
using PSPGP.Models;

namespace PSPGP.Cmdlets
{
    [Cmdlet(VerbsDiagnostic.Test, "PGP", DefaultParameterSetName = "File")]
    [OutputType(typeof(VerificationResult))]
    public class CmdletTestPGP : PSCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "Folder")]
        [Parameter(Mandatory = true, ParameterSetName = "File")]
        [Parameter(Mandatory = true, ParameterSetName = "String")]
        public string FilePathPublic { get; set; }

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

        protected override void ProcessRecord()
        {
            try
            {
                string resolvedPublicKey = PathResolver.Resolve(this, FilePathPublic);
                if (!File.Exists(resolvedPublicKey))
                {
                    WriteError(new ErrorRecord(new FileNotFoundException($"Public key doesn't exist {resolvedPublicKey}"), "PublicKeyNotFound", ErrorCategory.InvalidArgument, resolvedPublicKey));
                    return;
                }

                var encryptionKeys = new EncryptionKeys(new FileInfo(resolvedPublicKey));
                var pgp = new PGP(encryptionKeys);

                if (ParameterSetName == "Folder")
                {
                    string resolvedFolder = PathResolver.Resolve(this, FolderPath);
                    foreach (var file in Directory.GetFiles(resolvedFolder, "*", SearchOption.AllDirectories))
                    {
                        bool status = false;
                        string error = string.Empty;
                        try
                        {
                            status = pgp.VerifyFile(file);
                        }
                        catch (Exception ex)
                        {
                            error = ex.Message;
                        }
                        var result = new VerificationResult
                        {
                            FilePath = file,
                            Status = status,
                            Error = error
                        };
                        WriteObject(result);
                    }
                }
                else if (ParameterSetName == "File")
                {
                    string resolvedFile = PathResolver.Resolve(this, FilePath);
                    bool status = false;
                    string error = string.Empty;
                    try
                    {
                        status = pgp.VerifyFile(resolvedFile);
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                    var result = new VerificationResult
                    {
                        FilePath = resolvedFile,
                        Status = status,
                        Error = error
                    };
                    WriteObject(result);
                }
                else if (ParameterSetName == "String")
                {
                    try
                    {
                        pgp.VerifyArmoredString(String);
                    }
                    catch (Exception ex)
                    {
                        WriteError(new ErrorRecord(ex, "VerifyStringFailed", ErrorCategory.NotSpecified, null));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "TestPGPFailed", ErrorCategory.NotSpecified, null));
            }
        }
    }
}
