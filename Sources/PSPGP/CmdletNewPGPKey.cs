using Org.BouncyCastle.Bcpg;
using PgpCore;
using System;
using System.IO;
using System.Management.Automation;

namespace PSPGP;
[Cmdlet(VerbsCommon.New, "PGPKey", DefaultParameterSetName = "ClearText")]
public class CmdletNewPGPKey : PSCmdlet {
    [Parameter(Mandatory = true, ParameterSetName = "Strength")]
    [Parameter(Mandatory = true, ParameterSetName = "StrengthCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "ClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "Credential")]
    public string FilePathPublic { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = "Strength")]
    [Parameter(Mandatory = true, ParameterSetName = "StrengthCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "ClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "Credential")]
    public string FilePathPrivate { get; set; }

    [Parameter(ParameterSetName = "Strength")]
    [Parameter(ParameterSetName = "ClearText")]
    public string UserName { get; set; }

    [Parameter(ParameterSetName = "Strength")]
    [Parameter(ParameterSetName = "ClearText")]
    public string Password { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = "StrengthCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "Credential")]
    public PSCredential Credential { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = "Strength")]
    [Parameter(Mandatory = true, ParameterSetName = "StrengthCredential")]
    public int Strength { get; set; }

    [Parameter(Mandatory = true, ParameterSetName = "Strength")]
    [Parameter(Mandatory = true, ParameterSetName = "StrengthCredential")]
    public int Certainty { get; set; }

    [Parameter(ParameterSetName = "Strength")]
    [Parameter(ParameterSetName = "StrengthCredential")]
    public SwitchParameter EmitVersion { get; set; }

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
            var pgp = new PGP();
            PGPConfigurator.Configure(pgp, HashAlgorithm, CompressionAlgorithm, FileType, PgpSignatureType, PublicKeyAlgorithm, SymmetricKeyAlgorithm);

            string resolvedPublic = PathResolver.Resolve(this, FilePathPublic);
            string resolvedPrivate = PathResolver.Resolve(this, FilePathPrivate);

            string user = UserName;
            string pass = Password;
            if (Credential != null) {
                user = Credential.UserName;
                pass = Credential.GetNetworkCredential().Password;
            }

            if (ParameterSetName.StartsWith("Strength")) {
                pgp.GenerateKey(new FileInfo(resolvedPublic), new FileInfo(resolvedPrivate), user, pass, Strength, Certainty, EmitVersion.IsPresent);
            } else {
                pgp.GenerateKey(new FileInfo(resolvedPublic), new FileInfo(resolvedPrivate), user, pass);
            }
        } catch (Exception ex) {
            WriteError(new ErrorRecord(ex, "NewPGPKeyFailed", ErrorCategory.NotSpecified, null));
        }
    }
}