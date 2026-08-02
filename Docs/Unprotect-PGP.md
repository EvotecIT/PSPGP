---
external help file: PSPGP-help.xml
Module Name: PSPGP
online version: https://github.com/EvotecIT/PSPGP
schema: 2.0.0
---
# Unprotect-PGP
## SYNOPSIS
Removes PGP encryption from files or strings using a private key or symmetric passphrase.

## SYNTAX
### FolderClearText (Default)
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -FolderPath <string> -OutputFolderPath <string> [-Password <string>] [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### FolderCredential
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -Credential <pscredential> -FolderPath <string> -OutputFolderPath <string> [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### FileCredential
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -Credential <pscredential> -FilePath <string> -OutFilePath <string> [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### FileClearText
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -FilePath <string> -OutFilePath <string> [-Password <string>] [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### StringClearText
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -String <string> [-Password <string>] [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### StringCredential
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -Credential <pscredential> -String <string> [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### FolderVerifyCredential
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -FilePathPublic <string[]> -Credential <pscredential> -FolderPath <string> -OutputFolderPath <string> -Verify [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### FolderVerifyClearText
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -FilePathPublic <string[]> -FolderPath <string> -OutputFolderPath <string> -Verify [-Password <string>] [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### FileVerifyCredential
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -FilePathPublic <string[]> -Credential <pscredential> -FilePath <string> -OutFilePath <string> -Verify [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### FileVerifyClearText
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -FilePathPublic <string[]> -FilePath <string> -OutFilePath <string> -Verify [-Password <string>] [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### StringVerifyClearText
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -FilePathPublic <string[]> -String <string> -Verify [-Password <string>] [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### StringVerifyCredential
```powershell
Unprotect-PGP -FilePathPrivate <string[]> -FilePathPublic <string[]> -Credential <pscredential> -String <string> -Verify [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### FolderSymmetric
```powershell
Unprotect-PGP -SymmetricPassphrase <string> -FolderPath <string> -OutputFolderPath <string> [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### FileSymmetric
```powershell
Unprotect-PGP -SymmetricPassphrase <string> -FilePath <string> -OutFilePath <string> [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

### StringSymmetric
```powershell
Unprotect-PGP -SymmetricPassphrase <string> -String <string> [-IgnoreIntegrityCheckFailure] [<CommonParameters>]
```

## DESCRIPTION
Removes PGP encryption from files or strings using a private key or symmetric passphrase.

## EXAMPLES

### EXAMPLE 1
```powershell
Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -Password 'secret' -FolderPath $PSScriptRoot\Encoded -OutputFolderPath $PSScriptRoot\Decoded
```


### EXAMPLE 2
```powershell
Unprotect-PGP -FilePathPrivate $PSScriptRoot\Keys\PrivatePGP1.asc -Password 'secret' -String $Encrypted
```


### EXAMPLE 3
```powershell
Unprotect-PGP -SymmetricPassphrase 'SymmetricPass123!' -String $Encrypted
```


## PARAMETERS

### -Credential
Credential object with password for the private key.

```yaml
Type: PSCredential
Parameter Sets: FolderCredential, FileCredential, StringCredential, FolderVerifyCredential, FileVerifyCredential, StringVerifyCredential
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FilePath
Encrypted file to decrypt.

```yaml
Type: String
Parameter Sets: FileCredential, FileClearText, FileVerifyCredential, FileVerifyClearText, FileSymmetric
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FilePathPrivate
Private key file used to decrypt data.

```yaml
Type: String[]
Parameter Sets: FolderClearText, FolderCredential, FileCredential, FileClearText, StringClearText, StringCredential, FolderVerifyCredential, FolderVerifyClearText, FileVerifyCredential, FileVerifyClearText, StringVerifyClearText, StringVerifyCredential
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FilePathPublic
Public key files reserved for signed-and-encrypted verification workflows.

```yaml
Type: String[]
Parameter Sets: FolderVerifyCredential, FolderVerifyClearText, FileVerifyCredential, FileVerifyClearText, StringVerifyClearText, StringVerifyCredential
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FolderPath
Folder containing encrypted files.

```yaml
Type: String
Parameter Sets: FolderClearText, FolderCredential, FolderVerifyCredential, FolderVerifyClearText, FolderSymmetric
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -IgnoreIntegrityCheckFailure
Ignores modification-detection/integrity-check failures during decryption.

```yaml
Type: SwitchParameter
Parameter Sets: FolderClearText, FolderCredential, FileCredential, FileClearText, StringClearText, StringCredential, FolderVerifyCredential, FolderVerifyClearText, FileVerifyCredential, FileVerifyClearText, StringVerifyClearText, StringVerifyCredential, FolderSymmetric, FileSymmetric, StringSymmetric
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutFilePath
Output file path for decrypted data.

```yaml
Type: String
Parameter Sets: FileCredential, FileClearText, FileVerifyCredential, FileVerifyClearText, FileSymmetric
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputFolderPath
Destination folder for decrypted output.

```yaml
Type: String
Parameter Sets: FolderClearText, FolderCredential, FolderVerifyCredential, FolderVerifyClearText, FolderSymmetric
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Password
Password protecting the private key.

```yaml
Type: String
Parameter Sets: FolderClearText, FileClearText, StringClearText, FolderVerifyClearText, FileVerifyClearText, StringVerifyClearText
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -String
Encrypted text to decrypt.

```yaml
Type: String
Parameter Sets: StringClearText, StringCredential, StringVerifyClearText, StringVerifyCredential, StringSymmetric
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SymmetricPassphrase
Passphrase used for symmetric decryption.

```yaml
Type: String
Parameter Sets: FolderSymmetric, FileSymmetric, StringSymmetric
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Verify
Reserved for future signed-and-encrypted verification support.

```yaml
Type: SwitchParameter
Parameter Sets: FolderVerifyCredential, FolderVerifyClearText, FileVerifyCredential, FileVerifyClearText, StringVerifyClearText, StringVerifyCredential
Aliases: None
Possible values:

Required: True
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
