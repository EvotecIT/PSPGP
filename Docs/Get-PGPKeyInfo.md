---
external help file: PSPGP-help.xml
Module Name: PSPGP
online version: https://github.com/EvotecIT/PSPGP
schema: 2.0.0
---
# Get-PGPKeyInfo
## SYNOPSIS
Returns information about a PGP key such as algorithm, expiration and user IDs.

## SYNTAX
### __AllParameterSets
```powershell
Get-PGPKeyInfo -FilePath <string[]> [<CommonParameters>]
```

## DESCRIPTION
Returns information about a PGP key such as algorithm, expiration and user IDs.

## EXAMPLES

### EXAMPLE 1
```powershell
Get-PGPKeyInfo -FilePath $PSScriptRoot\Keys\PublicPGP1.asc
```


## PARAMETERS

### -FilePath
Paths to key files to inspect.

```yaml
Type: String[]
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: True (ByValue, ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

- `System.String[]`

## OUTPUTS

- `PSPGP.PGPKeyInfo`: Information about a PGP key file.

## RELATED LINKS

- None
