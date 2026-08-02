---
external help file: PSPGP-help.xml
Module Name: PSPGP
online version: https://github.com/EvotecIT/PSPGP
schema: 2.0.0
---
# New-PGPKey
## SYNOPSIS
Generates a new PGP key pair.

## SYNTAX
### ClearText (Default)
```powershell
New-PGPKey -FilePathPublic <string> -FilePathPrivate <string> [-UploadKeyServer <string>] [-UserName <string>] [-Password <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [<CommonParameters>]
```

### Strength
```powershell
New-PGPKey -FilePathPublic <string> -FilePathPrivate <string> -Strength <int> -Certainty <int> [-UploadKeyServer <string>] [-UserName <string>] [-Password <string>] [-EmitVersion] [-Armor <bool>] [-KeyExpirationInSeconds <long>] [-SignatureExpirationInSeconds <long>] [-HashAlgorithm <HashAlgorithmTag>] [-PreferredHashAlgorithm <HashAlgorithmTag[]>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-PreferredCompressionAlgorithm <CompressionAlgorithmTag[]>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-PreferredSymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag[]>] [<CommonParameters>]
```

### StrengthCredential
```powershell
New-PGPKey -FilePathPublic <string> -FilePathPrivate <string> -Credential <pscredential> -Strength <int> -Certainty <int> [-UploadKeyServer <string>] [-EmitVersion] [-Armor <bool>] [-KeyExpirationInSeconds <long>] [-SignatureExpirationInSeconds <long>] [-HashAlgorithm <HashAlgorithmTag>] [-PreferredHashAlgorithm <HashAlgorithmTag[]>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-PreferredCompressionAlgorithm <CompressionAlgorithmTag[]>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-PreferredSymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag[]>] [<CommonParameters>]
```

### Credential
```powershell
New-PGPKey -FilePathPublic <string> -FilePathPrivate <string> -Credential <pscredential> [-UploadKeyServer <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [<CommonParameters>]
```

## DESCRIPTION
Generates a new PGP key pair.

## EXAMPLES

### EXAMPLE 1
```powershell
New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -UserName 'user' -Password 'secret'
```


### EXAMPLE 2
```powershell
New-PGPKey -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -Strength 4096 -Certainty 24 -EmitVersion
```


## PARAMETERS

### -Armor
Controls whether generated key files are ASCII armored.

```yaml
Type: Boolean
Parameter Sets: Strength, StrengthCredential
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Certainty
Certainty value used when generating a key.

```yaml
Type: Int32
Parameter Sets: Strength, StrengthCredential
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CompressionAlgorithm
Optional compression algorithm used when generating keys.

```yaml
Type: Nullable`1
Parameter Sets: ClearText, Strength, StrengthCredential, Credential
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Credential
Credential object providing user name and password.

```yaml
Type: PSCredential
Parameter Sets: StrengthCredential, Credential
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -EmitVersion
Adds the PGP version notation to the key.

```yaml
Type: SwitchParameter
Parameter Sets: Strength, StrengthCredential
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FilePathPrivate
Path to the private key file to create.

```yaml
Type: String
Parameter Sets: ClearText, Strength, StrengthCredential, Credential
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FilePathPublic
Path to the public key file to create.

```yaml
Type: String
Parameter Sets: ClearText, Strength, StrengthCredential, Credential
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FileType
Defines the file type stored within the PGP package.

```yaml
Type: Nullable`1
Parameter Sets: ClearText, Strength, StrengthCredential, Credential
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -HashAlgorithm
Optional hash algorithm used when generating keys.

```yaml
Type: Nullable`1
Parameter Sets: ClearText, Strength, StrengthCredential, Credential
Aliases: HashAlgorithmTag
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -KeyExpirationInSeconds
Key expiration in seconds. Use zero for no expiration.

```yaml
Type: Int64
Parameter Sets: Strength, StrengthCredential
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Password
Password used to protect the private key.

```yaml
Type: String
Parameter Sets: ClearText, Strength
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PgpSignatureType
PGP signature type used when creating the key.

```yaml
Type: Nullable`1
Parameter Sets: ClearText, Strength, StrengthCredential, Credential
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PreferredCompressionAlgorithm
Preferred compression algorithms advertised by the generated key.

```yaml
Type: CompressionAlgorithmTag[]
Parameter Sets: Strength, StrengthCredential
Aliases: None
Possible values: Uncompressed, Zip, ZLib, BZip2

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PreferredHashAlgorithm
Preferred hash algorithms advertised by the generated key.

```yaml
Type: HashAlgorithmTag[]
Parameter Sets: Strength, StrengthCredential
Aliases: None
Possible values: MD5, Sha1, RipeMD160, DoubleSha, MD2, Tiger192, Haval5pass160, Sha256, Sha384, Sha512, Sha224, Sha3_256, Sha3_512, MD4, Sha3_224, Sha3_256_Old, Sha3_384, Sha3_512_Old, SM3

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PreferredSymmetricKeyAlgorithm
Preferred symmetric algorithms advertised by the generated key.

```yaml
Type: SymmetricKeyAlgorithmTag[]
Parameter Sets: Strength, StrengthCredential
Aliases: None
Possible values: Null, Idea, TripleDes, Cast5, Blowfish, Safer, Des, Aes128, Aes192, Aes256, Twofish, Camellia128, Camellia192, Camellia256

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PublicKeyAlgorithm
Public key algorithm used for key creation.

```yaml
Type: Nullable`1
Parameter Sets: ClearText, Strength, StrengthCredential, Credential
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SignatureExpirationInSeconds
Signature expiration in seconds. Use zero for no expiration.

```yaml
Type: Int64
Parameter Sets: Strength, StrengthCredential
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Strength
Key strength in bits.

```yaml
Type: Int32
Parameter Sets: Strength, StrengthCredential
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SymmetricKeyAlgorithm
Symmetric key algorithm used for encryption.

```yaml
Type: Nullable`1
Parameter Sets: ClearText, Strength, StrengthCredential, Credential
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UploadKeyServer
Key server URL to upload the generated public key.

```yaml
Type: String
Parameter Sets: ClearText, Strength, StrengthCredential, Credential
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UserName
User name associated with the generated key.

```yaml
Type: String
Parameter Sets: ClearText, Strength
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

- `None`

## OUTPUTS

- `None`

## RELATED LINKS

- None
