using Org.BouncyCastle.Bcpg;
using System;

namespace PSPGP;

public class PGPKeyInfo {
    public string FilePath { get; set; }
    public string[] UserIds { get; set; }
    public PublicKeyAlgorithmTag Algorithm { get; set; }
    public DateTime? Expiration { get; set; }
}
