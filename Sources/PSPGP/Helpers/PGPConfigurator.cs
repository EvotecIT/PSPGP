using Org.BouncyCastle.Bcpg;
using PgpCore;

namespace PSPGP.Helpers; 
public static class PGPConfigurator {
    public static void Configure(
        PGP pgp,
        HashAlgorithmTag? hashAlgorithm,
        CompressionAlgorithmTag? compressionAlgorithm,
        PgpCore.Enums.PGPFileType? fileType,
        int? signatureType,
        PublicKeyAlgorithmTag? publicKeyAlgorithm,
        SymmetricKeyAlgorithmTag? symmetricKeyAlgorithm) {
        if (hashAlgorithm.HasValue) pgp.HashAlgorithmTag = hashAlgorithm.Value;
        if (compressionAlgorithm.HasValue) pgp.CompressionAlgorithm = compressionAlgorithm.Value;
        if (fileType.HasValue) pgp.FileType = fileType.Value;
        if (signatureType.HasValue) pgp.PgpSignatureType = signatureType.Value;
        if (publicKeyAlgorithm.HasValue) pgp.PublicKeyAlgorithm = publicKeyAlgorithm.Value;
        if (symmetricKeyAlgorithm.HasValue) pgp.SymmetricKeyAlgorithm = symmetricKeyAlgorithm.Value;
    }
}