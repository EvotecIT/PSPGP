function Protect-PGP {
    [cmdletBinding(DefaultParameterSetName = 'File')]
    param(
        [Parameter(Mandatory, ParameterSetName = 'Folder')]
        [Parameter(Mandatory, ParameterSetName = 'File')]
        [Parameter(Mandatory, ParameterSetName = 'String')]
        [string] $FilePathPublic,

        [Parameter(Mandatory, ParameterSetName = 'Folder')][string] $FolderPath,
        [Parameter(ParameterSetName = 'Folder')][string] $OutputFolderPath,

        [Parameter(Mandatory, ParameterSetName = 'File')][string] $FilePath,
        [Parameter(ParameterSetName = 'File')][string] $OutFilePath,

        [Parameter(Mandatory, ParameterSetName = 'String')][string] $String
    )

    if (Test-Path -LiteralPath $FilePathPublic) {
        $PublicKey = [System.IO.FileInfo]::new($FilePathPublic)
    } else {
        if ($PSBoundParameters.ErrorAction -eq 'Stop') {
            throw
        } else {
            Write-Warning -Message "Protect-PGP - Public key doesn't exists $($FilePathPublic): $($_.Exception.Message)"
            return
        }
    }
    try {
        $EncryptionKeys = [PgpCore.EncryptionKeys]::new($PublicKey)
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
        foreach ($File in Get-ChildItem -LiteralPath $FolderPath -Recurse:$Recursive) {
            try {
                if ($OutputFolderPath) {
                    $OutputFile = [io.Path]::Combine($OutputFolderPath, "$($File.Name).pgp")
                    $PGP.EncryptFile($File.FullName, $OutputFile)
                } else {
                    $PGP.EncryptFile($File.FullName, "$($File.FullName).pgp")
                }
            } catch {
                if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                    throw
                } else {
                    Write-Warning -Message "Protect-PGP - Can't encrypt file $($File.FuleName): $($_.Exception.Message)"
                    return
                }
            }
        }
    } elseif ($FilePath) {
        try {
            if ($OutFilePath) {
                $PGP.EncryptFile($FilePath, "$OutFilePath")
            } else {
                $PGP.EncryptFile($FilePath, "$($FilePath).pgp")
            }
        } catch {
            if ($PSBoundParameters.ErrorAction -eq 'Stop') {
                throw
            } else {
                Write-Warning -Message "Protect-PGP - Can't encrypt file $($File.FuleName): $($_.Exception.Message)"
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
                Write-Warning -Message "Protect-PGP - Can't encrypt file $($File.FuleName): $($_.Exception.Message)"
            }
        }
    }
}