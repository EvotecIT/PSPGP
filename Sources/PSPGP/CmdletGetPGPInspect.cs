using System.IO;
using System.Management.Automation;
using PgpCore;
using PgpCore.Models;

namespace PSPGP;

/// <summary>
/// Inspects PGP content and returns message metadata.
/// <example>
/// <code>
/// $Signature = Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'secret' -String 'Signed text'
/// Get-PGPInspect -String $Signature
/// </code>
/// </example>
/// </summary>
[Cmdlet(VerbsCommon.Get, "PGPInspect", DefaultParameterSetName = "File")]
[OutputType(typeof(PGPInspectInfo))]
public class CmdletGetPGPInspect : PSCmdlet {
    /// <summary>Folder containing PGP files to inspect.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Folder")]
    public string FolderPath { get; set; }

    /// <summary>File to inspect.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "File")]
    public string FilePath { get; set; }

    /// <summary>Armored message content to inspect.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "String")]
    public string String { get; set; }

    /// <summary>
    /// Inspects the selected input and writes message metadata.
    /// </summary>
    protected override void ProcessRecord() {
        try {
            var pgp = new PGP();

            if (ParameterSetName == "Folder") {
                string resolvedFolder = PathResolver.Resolve(this, FolderPath);
                foreach (string file in Directory.GetFiles(resolvedFolder, "*", SearchOption.AllDirectories)) {
                    WriteObject(ToInfo(file, pgp.Inspect(new FileInfo(file))));
                }
            } else if (ParameterSetName == "File") {
                string resolvedFile = PathResolver.Resolve(this, FilePath);
                WriteObject(ToInfo(resolvedFile, pgp.Inspect(new FileInfo(resolvedFile))));
            } else {
                WriteObject(ToInfo(null, pgp.Inspect(String)));
            }
        } catch (System.Exception ex) {
            if (ex is System.NullReferenceException) {
                ex = new System.NotSupportedException(
                    "PgpCore inspect currently fails for some encrypted messages. Signed content inspection works, but encrypted-message inspection is limited by the upstream library.",
                    ex);
            }

            WriteError(new ErrorRecord(ex, "GetPGPInspectFailed", ErrorCategory.NotSpecified, null));
        }
    }

    private static PGPInspectInfo ToInfo(string sourcePath, PgpInspectResult result) {
        return new PGPInspectInfo {
            SourcePath = sourcePath,
            IsArmored = result.IsArmored,
            MessageHeaders = result.MessageHeaders,
            Version = result.Version,
            Comment = result.Comment,
            IsCompressed = result.IsCompressed,
            IsEncrypted = result.IsEncrypted,
            IsIntegrityProtected = result.IsIntegrityProtected,
            IsSigned = result.IsSigned,
            SymmetricKeyAlgorithm = result.SymmetricKeyAlgorithm,
            FileName = result.FileName,
            ModificationDateTime = result.ModificationDateTime
        };
    }
}
