namespace PSPGP;

/// <summary>
/// Represents the outcome of a signature verification operation.
/// </summary>
public class VerificationResult {
    /// <summary>Path to the file that was verified.</summary>
    public string FilePath { get; set; }

    /// <summary>Indicates whether the verification succeeded.</summary>
    public bool Status { get; set; }

    /// <summary>Error message when verification failed.</summary>
    public string Error { get; set; }

    /// <summary>Public key used to verify the signature.</summary>
    public string Signer { get; set; }
}
