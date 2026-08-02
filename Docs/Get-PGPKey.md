---
external help file: PSPGP-help.xml
Module Name: PSPGP
online version: https://github.com/EvotecIT/PSPGP
schema: 2.0.0
---
# Get-PGPKey
## SYNOPSIS
Downloads a public key from a key server.

## SYNTAX
### __AllParameterSets
```powershell
Get-PGPKey -KeyServer <string> -Search <string> [-OutFilePath <string>] [<CommonParameters>]
```

## DESCRIPTION
Downloads a public key from a key server.

## EXAMPLES

### EXAMPLE 1
```powershell
Get-PGPKey -KeyServer "https://keys.example.com" -Search "user@example.com" -OutFilePath "key.asc"
```


## PARAMETERS

### -KeyServer
URL of the key server.

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
File path where the downloaded key is stored.

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

### -Search
Search string identifying the key.

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
