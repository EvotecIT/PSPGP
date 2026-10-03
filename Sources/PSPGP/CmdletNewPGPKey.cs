using Org.BouncyCastle.Bcpg;
using PgpCore;
using System;
using System.IO;
using System.Management.Automation;
using System.Threading.Tasks;

namespace PSPGP;
/// <summary>
/// <para>Generates a new PGP key pair.</para>
/// </summary>
/// <example>
/// <code>
/// New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -UserName 'user' -Password 'secret'
/// </code>
/// </example>
/// <example>
/// <code>
/// New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -Strength 4096 -Certainty 24 -EmitVersion
/// </code>
/// </example>
[Cmdlet(VerbsCommon.New, "PGPKey", SupportsShouldProcess = true, DefaultParameterSetName = "ClearText")]
public class CmdletNewPGPKey : AsyncPSCmdlet {
    /// <summary>Path to the public key file to create.</summary>
    [Parameter(Mandatory = true)]
    public string FilePathPublic { get; set; }

    /// <summary>Path to the private key file to create.</summary>
    [Parameter(Mandatory = true)]
    public string FilePathPrivate { get; set; }

    /// <summary>Key server URL to upload the generated public key.</summary>
    [Parameter]
    public string UploadKeyServer { get; set; }

    /// <summary>Explicitly permits replacing existing public and private key files.</summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    /// <summary>User name associated with the generated key.</summary>
    [Parameter(ParameterSetName = "ClearText")]
    public string UserName { get; set; }

    /// <summary>Password used to protect the private key.</summary>
    [Parameter(ParameterSetName = "ClearText")]
    public string Password { get; set; }

    /// <summary>Credential object providing user name and password.</summary>
    [Parameter(Mandatory = true, ParameterSetName = "Credential")]
    public PSCredential Credential { get; set; }

    /// <summary>Key strength in bits.</summary>
    [Parameter]
    public int Strength { get; set; } = 3072;

    /// <summary>Certainty value used when generating a key.</summary>
    [Parameter]
    public int Certainty { get; set; } = 24;

    /// <summary>Adds the PGP version notation to the key.</summary>
    [Parameter]
    public SwitchParameter EmitVersion { get; set; }

    /// <summary>Controls whether generated key files are ASCII armored.</summary>
    [Parameter]
    public bool Armor { get; set; } = true;

    /// <summary>Key expiration in seconds. Use zero for no expiration.</summary>
    [Parameter]
    public long KeyExpirationInSeconds { get; set; }

    /// <summary>Signature expiration in seconds. Use zero for no expiration.</summary>
    [Parameter]
    public long SignatureExpirationInSeconds { get; set; }

    /// <summary>Optional hash algorithm used when generating keys.</summary>
    [Parameter]
    [Alias("HashAlgorithmTag")]
    public HashAlgorithmTag? HashAlgorithm { get; set; }

    /// <summary>Preferred hash algorithms advertised by the generated key.</summary>
    [Parameter]
    public HashAlgorithmTag[] PreferredHashAlgorithm { get; set; }

    /// <summary>Optional compression algorithm used when generating keys.</summary>
    [Parameter]
    public CompressionAlgorithmTag? CompressionAlgorithm { get; set; }

    /// <summary>Preferred compression algorithms advertised by the generated key.</summary>
    [Parameter]
    public CompressionAlgorithmTag[] PreferredCompressionAlgorithm { get; set; }

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

    /// <summary>Preferred symmetric algorithms advertised by the generated key.</summary>
    [Parameter]
    public SymmetricKeyAlgorithmTag[] PreferredSymmetricKeyAlgorithm { get; set; }

    /// <summary>Returns the successfully written key file or key-pair files.</summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    /// <summary>
    /// Generates a new key pair based on the provided
    /// parameters and optionally uploads the public key.
    /// </summary>
    protected override async Task ProcessRecordAsync() {
        try {
            using var pgp = new PGP();
            if (!string.IsNullOrEmpty(UploadKeyServer)) KeyServerHelper.ValidateServer(new Uri(UploadKeyServer));
            PGPConfigurator.Configure(pgp, HashAlgorithm, CompressionAlgorithm, FileType, PgpSignatureType, PublicKeyAlgorithm, SymmetricKeyAlgorithm);

            string resolvedPublic = PathResolver.Resolve(this, FilePathPublic);
            string resolvedPrivate = PathResolver.Resolve(this, FilePathPrivate);

            string user = UserName;
            string pass = Password;
            if (Credential != null) {
                user = Credential.UserName;
                pass = Credential.GetNetworkCredential().Password;
            }

            if (!Armor && !string.IsNullOrEmpty(UploadKeyServer)) {
                throw new InvalidOperationException("New-PGPKey requires armored public key output when UploadKeyServer is used.");
            }

            if (!Force.IsPresent && (File.Exists(resolvedPublic) || File.Exists(resolvedPrivate))) {
                throw new IOException("Key files already exist. Use -Force to replace them explicitly.");
            }
            if (!ShouldProcess($"{resolvedPublic}; {resolvedPrivate}", "Generate PGP key pair")) return;
            Directory.CreateDirectory(Path.GetDirectoryName(resolvedPublic));
            Directory.CreateDirectory(Path.GetDirectoryName(resolvedPrivate));
            pgp.GenerateKey(new FileInfo(resolvedPublic), new FileInfo(resolvedPrivate), user, pass,
                Strength, Certainty, Armor, EmitVersion.IsPresent, KeyExpirationInSeconds, SignatureExpirationInSeconds,
                PreferredCompressionAlgorithm ?? ToArray(CompressionAlgorithm),
                PreferredHashAlgorithm ?? ToArray(HashAlgorithm),
                PreferredSymmetricKeyAlgorithm ?? ToArray(SymmetricKeyAlgorithm));

            if (!string.IsNullOrEmpty(UploadKeyServer) && ShouldProcess(UploadKeyServer, "Upload public PGP key")) {
                string keyData = File.ReadAllText(resolvedPublic);
                await KeyServerHelper.UploadKeyAsync(new Uri(UploadKeyServer), keyData, CancelToken).ConfigureAwait(false);
            }
            if (PassThru.IsPresent) {
                WriteObject(new FileInfo(resolvedPublic));
                WriteObject(new FileInfo(resolvedPrivate));
            }
        } catch (OperationCanceledException) when (CancelToken.IsCancellationRequested) { throw; }
        catch (Exception ex) when (ex is not PipelineStoppedException && ex is not ActionPreferenceStopException) {
            WriteError(PgpExceptionHelper.CreateErrorRecord(ex, "NewPGPKeyFailed"));
        }
    }

    private static T[] ToArray<T>(T? value) where T : struct {
        return value.HasValue ? new[] { value.Value } : null;
    }
}
