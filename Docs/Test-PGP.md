---
external help file: PSPGP-help.xml
Module Name: PSPGP
online version: https://github.com/EvotecIT/PSPGP
schema: 2.0.0
---
# Test-PGP
## SYNOPSIS
Verifies PGP signatures for files, folders or strings.

## SYNTAX
### File (Default)
```powershell
Test-PGP -FilePathPublic <string[]> -FilePath <string> [-SignaturePath <string>] [-OutFilePath <string>] [-ThrowIfEncrypted] [-ClearSigned] [<CommonParameters>]
```

### Folder
```powershell
Test-PGP -FilePathPublic <string[]> -FolderPath <string> [-OutputFolderPath <string>] [-ThrowIfEncrypted] [-ClearSigned] [<CommonParameters>]
```

### String
```powershell
Test-PGP -FilePathPublic <string[]> -String <string> [-Signature <string>] [-ThrowIfEncrypted] [-ClearSigned] [<CommonParameters>]
```

## DESCRIPTION
Verifies PGP signatures for files, folders or strings.

## EXAMPLES

### EXAMPLE 1
```powershell
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $ProtectedString
```


### EXAMPLE 2
```powershell
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FolderPath $PSScriptRoot\Encoded
```


### EXAMPLE 3
```powershell
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -FilePath $PSScriptRoot\Test\Test1.txt -SignaturePath $PSScriptRoot\Test\Test1.txt.sig
```


### EXAMPLE 4
```powershell
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $ClearSigned -ClearSigned
```


### EXAMPLE 5
```powershell
Test-PGP -FilePathPublic $PSScriptRoot\Keys\PublicPGP1.asc -String $ProtectedString -ThrowIfEncrypted
```


## PARAMETERS

### -ClearSigned
Verifies clear-signed content instead of regular signed content.

```yaml
Type: SwitchParameter
Parameter Sets: File, Folder, String
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FilePath
File path to verify.

```yaml
Type: String
Parameter Sets: File
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FilePathPublic
Public key file used to verify signatures.

```yaml
Type: String[]
Parameter Sets: File, Folder, String
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FolderPath
Folder containing files to verify.

```yaml
Type: String
Parameter Sets: Folder
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutFilePath
Output path for verified clear content.

```yaml
Type: String
Parameter Sets: File
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputFolderPath
Destination folder for verified clear content.

```yaml
Type: String
Parameter Sets: Folder
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Signature
Detached signature for the string input.

```yaml
Type: String
Parameter Sets: String
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SignaturePath
Detached signature file for the input file.

```yaml
Type: String
Parameter Sets: File
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -String
Signed text or original text when Signature is provided.

```yaml
Type: String
Parameter Sets: String
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ThrowIfEncrypted
Throws when encrypted content is passed to verify methods.

```yaml
Type: SwitchParameter
Parameter Sets: File, Folder, String
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

- `PSPGP.VerificationResult`: Represents the outcome of a signature verification operation.

## RELATED LINKS

- None
