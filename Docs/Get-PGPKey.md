---
external help file: PSPGP-help.xml
Module Name: PSPGP
online version: https://github.com/EvotecIT/PSPGP
schema: 2.0.0
---
# Get-PGPKey
## SYNOPSIS
Downloads validated public certificates from an HTTPS key server.

## SYNTAX
### __AllParameterSets
```powershell
Get-PGPKey -KeyServer <string> -Search <string> [-ExpectedFingerprint <string>] [-OutFilePath <string>] [-PassThru] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Downloads validated public certificates from an HTTPS key server.

## EXAMPLES

### EXAMPLE 1
```powershell
Get-PGPKey -KeyServer 'Value' -Search 'Value'
```


## PARAMETERS

### -ExpectedFingerprint
Optional complete primary fingerprint obtained through an independently trusted channel.

```yaml
Type: String
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -KeyServer
HTTPS URL of the key server.

```yaml
Type: String
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutFilePath
Optional destination file, replaced only after certificate validation.

```yaml
Type: String
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru
Returns the successfully written key file or key-pair files.

```yaml
Type: SwitchParameter
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Search
Search string identifying the key; a user ID or short key ID does not establish trust.

```yaml
Type: String
Parameter Sets: __AllParameterSets
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
