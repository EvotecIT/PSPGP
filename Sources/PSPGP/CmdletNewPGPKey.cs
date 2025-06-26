using Org.BouncyCastle.Bcpg;
using PgpCore;
using System;
using System.IO;
using System.Management.Automation;

namespace PSPGP;
/// <summary>
/// <para>Generates a new PGP key pair.</para>
/// <example>
/// <code>
/// New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP.asc -UserName 'user' -Password 'secret'
/// </code>
/// </example>
/// <example>
/// <code>
/// New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP.asc -Strength 4096 -Certainty 8 -EmitVersion
/// </code>
/// </example>
/// </summary>
[Cmdlet(VerbsCommon.New, "PGPKey", DefaultParameterSetName = "ClearText")]
public class CmdletNewPGPKey : PSCmdlet {
    /// <summary>Path to the public key file to create.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Strength")]
    [Parameter(Mandatory = true, ParameterSetName = "StrengthCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "ClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "Credential")]
    public string FilePathPublic { get; set; }

    /// <summary>Path to the private key file to create.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Strength")]
    [Parameter(Mandatory = true, ParameterSetName = "StrengthCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "ClearText")]
    [Parameter(Mandatory = true, ParameterSetName = "Credential")]
    public string FilePathPrivate { get; set; }

    /// <summary>User name associated with the generated key.</summary>
    [Parameter(ParameterSetName = "Strength")]
    [Parameter(ParameterSetName = "ClearText")]
    public string UserName { get; set; }

    /// <summary>Password used to protect the private key.</summary>
    [Parameter(ParameterSetName = "Strength")]
    [Parameter(ParameterSetName = "ClearText")]
    public string Password { get; set; }

    /// <summary>Credential object providing user name and password.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "StrengthCredential")]
    [Parameter(Mandatory = true, ParameterSetName = "Credential")]
    public PSCredential Credential { get; set; }

    /// <summary>Key strength in bits.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Strength")]
    [Parameter(Mandatory = true, ParameterSetName = "StrengthCredential")]
    public int Strength { get; set; }

    /// <summary>Certainty value used when generating a key.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Strength")]
    [Parameter(Mandatory = true, ParameterSetName = "StrengthCredential")]
    public int Certainty { get; set; }

    /// <summary>Adds the PGP version notation to the key.</summary>
    [Parameter(ParameterSetName = "Strength")]
    [Parameter(ParameterSetName = "StrengthCredential")]
    public SwitchParameter EmitVersion { get; set; }

    /// <summary>Optional hash algorithm used when generating keys.</summary>
    [Parameter]
    [Alias("HashAlgorithmTag")]
    public HashAlgorithmTag? HashAlgorithm { get; set; }

    /// <summary>Optional compression algorithm used when generating keys.</summary>
    [Parameter]
    public CompressionAlgorithmTag? CompressionAlgorithm { get; set; }

    /// <summary>Defines the file type stored within the PGP package.</summary>
    [Parameter]
    public PgpCore.Enums.PGPFileType? FileType { get; set; }

    /// <summary>PGP signature type used when creating the key.</summary>
    [Parameter]
    public int? PgpSignatureType { get; set; }

    /// <summary>Public key algorithm used for key creation.</summary>
    [Parameter]
    public PublicKeyAlgorithmTag? PublicKeyAlgorithm { get; set; }

    /// <summary>Symmetric key algorithm used for encryption.</summary>
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