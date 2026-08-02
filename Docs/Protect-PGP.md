---
external help file: PSPGP-help.xml
Module Name: PSPGP
online version: https://github.com/EvotecIT/PSPGP
schema: 2.0.0
---
# Protect-PGP
## SYNOPSIS
Encrypts or signs files, folders or strings using one or more public keys.
Use -SignOnly or the Sign* parameter sets to create signatures
without encryption. Add -Detached to create a separate detached signature.

## SYNTAX
### File (Default)
```powershell
Protect-PGP -FilePathPublic <string[]> -FilePath <string> [-OutFilePath <string>] [-SignKey <FileInfo>] [-SignPassword <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [<CommonParameters>]
```

### Folder
```powershell
Protect-PGP -FilePathPublic <string[]> -FolderPath <string> [-OutputFolderPath <string>] [-SignKey <FileInfo>] [-SignPassword <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [<CommonParameters>]
```

### String
```powershell
Protect-PGP -FilePathPublic <string[]> -String <string> [-SignKey <FileInfo>] [-SignPassword <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [<CommonParameters>]
```

### SignFolder
```powershell
Protect-PGP -FolderPath <string> -SignKey <FileInfo> -SignOnly [-FilePathPublic <string[]>] [-OutputFolderPath <string>] [-SignPassword <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [-Detached] [<CommonParameters>]
```

### SignFile
```powershell
Protect-PGP -FilePath <string> -SignKey <FileInfo> -SignOnly [-FilePathPublic <string[]>] [-OutFilePath <string>] [-SignPassword <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [-Detached] [<CommonParameters>]
```

### SignString
```powershell
Protect-PGP -String <string> -SignKey <FileInfo> -SignOnly [-FilePathPublic <string[]>] [-SignPassword <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [-Detached] [<CommonParameters>]
```

### ClearSignFolder
```powershell
Protect-PGP -FolderPath <string> -SignKey <FileInfo> -ClearSign [-OutputFolderPath <string>] [-SignPassword <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [<CommonParameters>]
```

### SymmetricFolder
```powershell
Protect-PGP -FolderPath <string> -SymmetricPassphrase <string> [-OutputFolderPath <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [<CommonParameters>]
```

### ClearSignFile
```powershell
Protect-PGP -FilePath <string> -SignKey <FileInfo> -ClearSign [-OutFilePath <string>] [-SignPassword <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [<CommonParameters>]
```

### SymmetricFile
```powershell
Protect-PGP -FilePath <string> -SymmetricPassphrase <string> [-OutFilePath <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [<CommonParameters>]
```

### ClearSignString
```powershell
Protect-PGP -String <string> -SignKey <FileInfo> -ClearSign [-SignPassword <string>] [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [<CommonParameters>]
```

### SymmetricString
```powershell
Protect-PGP -String <string> -SymmetricPassphrase <string> [-HashAlgorithm <HashAlgorithmTag>] [-CompressionAlgorithm <CompressionAlgorithmTag>] [-FileType <PGPFileType>] [-PgpSignatureType <int>] [-PublicKeyAlgorithm <PublicKeyAlgorithmTag>] [-SymmetricKeyAlgorithm <SymmetricKeyAlgorithmTag>] [-Armor <bool>] [-WithIntegrityCheck <bool>] [-LiteralFileName <string>] [-Headers <hashtable>] [-OldFormat] [-AddVersionHeader] [<CommonParameters>]
```

## DESCRIPTION
Encrypts or signs files, folders or strings using one or more public keys.
Use -SignOnly or the Sign* parameter sets to create signatures
without encryption. Add -Detached to create a separate detached signature.

## EXAMPLES

### EXAMPLE 1
```powershell
Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FolderPath $PSScriptRoot\Test -OutputFolderPath $PSScriptRoot\Encoded
```


### EXAMPLE 2
```powershell
Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePath $PSScriptRoot\Test\Test1.txt -OutFilePath $PSScriptRoot\Encoded\Test1.txt.pgp
```


### EXAMPLE 3
```powershell
Protect-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String "Sensitive text"
```


### EXAMPLE 4
```powershell
Protect-PGP -SymmetricPassphrase 'SymmetricPass123!' -String 'Sensitive text'
```


### EXAMPLE 5
```powershell
Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'secret' -String "Signed content"
```


### EXAMPLE 6
```powershell
Protect-PGP -ClearSign -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'secret' -String "Human readable signed content"
```


## PARAMETERS

### -AddVersionHeader
Adds the PGP version header to generated armored content.

```yaml
Type: SwitchParameter
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Armor
Controls whether file output is armored.

```yaml
Type: Boolean
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ClearSign
Creates a clear-signed message that remains human readable.

```yaml
Type: SwitchParameter
Parameter Sets: ClearSignFolder, ClearSignFile, ClearSignString
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CompressionAlgorithm
Optional compression algorithm for encryption.

```yaml
Type: Nullable`1
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Detached
Creates a detached signature over the original input.

```yaml
Type: SwitchParameter
Parameter Sets: SignFolder, SignFile, SignString
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FilePath
File to encrypt when using the File parameter set.

```yaml
Type: String
Parameter Sets: File, SignFile, ClearSignFile, SymmetricFile
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FilePathPublic
Public key files used for encryption.

```yaml
Type: String[]
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FileType
Type of data being encrypted.

```yaml
Type: Nullable`1
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FolderPath
Folder to encrypt when using the Folder parameter set.

```yaml
Type: String
Parameter Sets: Folder, SignFolder, ClearSignFolder, SymmetricFolder
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -HashAlgorithm
Optional hash algorithm for encryption.

```yaml
Type: Nullable`1
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
Aliases: HashAlgorithmTag
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Headers
Optional armored headers added to generated content.

```yaml
Type: Hashtable
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LiteralFileName
Optional literal file name embedded in the PGP payload.

```yaml
Type: String
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
Aliases: Name
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OldFormat
Uses the legacy packet format when set.

```yaml
Type: SwitchParameter
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutFilePath
Output file path for the encrypted file.

```yaml
Type: String
Parameter Sets: File, SignFile, ClearSignFile, SymmetricFile
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputFolderPath
Destination folder for encrypted files.

```yaml
Type: String
Parameter Sets: Folder, SignFolder, ClearSignFolder, SymmetricFolder
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PgpSignatureType
PGP signature type when signing data.

```yaml
Type: Nullable`1
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PublicKeyAlgorithm
Public key algorithm used during encryption.

```yaml
Type: Nullable`1
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SignKey
Private key used for signing data. Mandatory when using the
Sign* parameter sets or the -SignOnly switch.

```yaml
Type: FileInfo
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, ClearSignFile, ClearSignString
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SignOnly
When specified, only a signature is produced instead of encrypting
the input. This parameter is automatically implied when using the
Sign* parameter sets.

```yaml
Type: SwitchParameter
Parameter Sets: SignFolder, SignFile, SignString
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SignPassword
Password for the signing private key.

```yaml
Type: String
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, ClearSignFile, ClearSignString
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -String
String content to encrypt.

```yaml
Type: String
Parameter Sets: String, SignString, ClearSignString, SymmetricString
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SymmetricKeyAlgorithm
Symmetric key algorithm used during encryption.

```yaml
Type: Nullable`1
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SymmetricPassphrase
Passphrase used for symmetric encryption.

```yaml
Type: String
Parameter Sets: SymmetricFolder, SymmetricFile, SymmetricString
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WithIntegrityCheck
Controls whether an integrity check packet is added.

```yaml
Type: Boolean
Parameter Sets: File, Folder, String, SignFolder, SignFile, SignString, ClearSignFolder, SymmetricFolder, ClearSignFile, SymmetricFile, ClearSignString, SymmetricString
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
