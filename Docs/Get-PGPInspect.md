---
external help file: PSPGP-help.xml
Module Name: PSPGP
online version: https://github.com/EvotecIT/PSPGP
schema: 2.0.0
---
# Get-PGPInspect
## SYNOPSIS
Inspects PGP content and returns message metadata.

## SYNTAX
### File (Default)
```powershell
Get-PGPInspect -FilePath <string> [<CommonParameters>]
```

### Folder
```powershell
Get-PGPInspect -FolderPath <string> [<CommonParameters>]
```

### String
```powershell
Get-PGPInspect -String <string> [<CommonParameters>]
```

## DESCRIPTION
Inspects PGP content and returns message metadata.

## EXAMPLES

### EXAMPLE 1
```powershell
$Signature = Protect-PGP -SignOnly -SignKey $PSScriptRoot\Keys\PrivatePGP1.asc -SignPassword 'secret' -String 'Signed text'
Get-PGPInspect -String $Signature
```


## PARAMETERS

### -FilePath
File to inspect.

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

### -FolderPath
Folder containing PGP files to inspect.

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

### -String
Armored message content to inspect.

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

- `None`

## OUTPUTS

- `PSPGP.PGPInspectInfo`: Represents the metadata extracted from a PGP message.

## RELATED LINKS

- None
