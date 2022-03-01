function Protect-PGP {
    [cmdletBinding(DefaultParameterSetName = 'File')]
    param(
        [Parameter(Mandatory, ParameterSetName = 'Folder')]
        [Parameter(Mandatory, ParameterSetName = 'File')]
        [Parameter(Mandatory, ParameterSetName = 'String')]
        [string[]] $FilePathPublic,

        [Parameter(Mandatory, ParameterSetName = 'Folder')][string] $FolderPath,
        [Parameter(ParameterSetName = 'Folder')][string] $OutputFolderPath,

        [Parameter(Mandatory, ParameterSetName = 'File')][string] $FilePath,
        [Parameter(ParameterSetName = 'File')][string] $OutFilePath,

        [Parameter(Mandatory, ParameterSetName = 'String')][string] $String
    )
    $PublicKeys =  [System.Collections.Generic.List[System.IO.FileInfo]]::new()
    foreach ($FilePathPubc in $FilePathPublic) {
        if (Test-Path -LiteralPath $FilePathPubc) {
            $PublicKeys.Add([System.IO.FileInfo]::new($FilePathPubc))
        } else {
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Protect-PGP - Public key doesn't exists $($FilePathPubc): $($_.Exception.Message)"
                return
            }
        }
    }
    try {
        $EncryptionKeys = [PgpCore.EncryptionKeys]::new($PublicKeys)
        $PGP = [PgpCore.PGP]::new($EncryptionKeys)
    } catch {
        if ($PSBoundParameters.ErrorAction -eq 'Stop') {
            throw
        } else {
            Write-Warning -Message "Protect-PGP - Can't encrypt files because: $($_.Exception.Message)"
            return
        }
    }
    if ($FolderPath) {
        $ResolvedFolderPath = Resolve-Path -Path $FolderPath
        foreach ($File in Get-ChildItem -LiteralPath $ResolvedFolderPath.Path -Recurse:$Recursive) {
            try {
                if ($OutputFolderPath) {
                    $ResolvedOutputFolder = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutputFolderPath)
                    $OutputFile = [io.Path]::Combine($ResolvedOutputFolder, "$($File.Name).pgp")
                    $PGP.EncryptFile($File.FullName, $OutputFile)
                } else {
                    $PGP.EncryptFile($File.FullName, "$($File.FullName).pgp")
                }
            } catch {
                if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                    throw
                } else {
                    Write-Warning -Message "Protect-PGP - Can't encrypt file $($File.FullName): $($_.Exception.Message)"
                    return
                }
            }
        }
    } elseif ($FilePath) {
        try {
            $ResolvedFilePath = Resolve-Path -Path $FilePath
            if ($OutFilePath) {
                $ResolvedOutFilePath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutFilePath)
                $PGP.EncryptFile($ResolvedFilePath.Path, "$($ResolvedOutFilePath)")
            } else {
                $PGP.EncryptFile($ResolvedFilePath.Path, "$($ResolvedFilePath.Path).pgp")
            }
        } catch {
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Protect-PGP - Can't encrypt file $($FilePath): $($_.Exception.Message)"
                return
            }
        }
    } elseif ($String) {
        try {
            $PGP.EncryptArmoredString($String)
        } catch {
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Protect-PGP - Can't encrypt string: $($_.Exception.Message)"
            }
        }
    }
}